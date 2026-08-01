namespace Simple.Indicators.UnitTest.IndicatorTests;

using Simple.Indicators.BR;
using System;

/// <summary>
/// Verdade de referência: Calculadora do Cidadão do Banco Central, Correção de valores.
/// Metodologia: https://www3.bcb.gov.br/CALCIDADAO/publico/metodologiaCorrigirIndice.do
/// <para>
/// "São usados no cálculo os índices da data inicial e da data final. Assim sendo, caso deseje
/// a correção por apenas um mês, o usuário deve informar a data inicial igual à data final."
/// </para>
/// </summary>
public class CalculadoraCidadaoBCBTests
{
    /// <summary>
    /// Exemplos publicados na própria página de metodologia do BCB.
    /// </summary>
    [Theory]
    [InlineData(2003, 1, 2003, 1, 1.0225)] // Exemplo 1: IPCA, mês único
    [InlineData(2003, 1, 2003, 12, 1.092999)] // Exemplo 2: IPCA em 2003 (BCB trunca em 1,0929994)
    public void ComputeRangeFor_IPCA_MatchesPublishedExamples(int y1, int m1, int y2, int m2, double fator)
    {
        var pct = CalculadoraCidadaoBCB.ComputeRangeFor<IPCA>(new DateTime(y1, m1, 1), new DateTime(y2, m2, 1));

        Assert.Equal((decimal)fator, Math.Round(1 + (pct / 100), 6));
    }

    [Theory]
    [InlineData(1989, 1, 1989, 5, 2.1046)] // Exemplo 3: INPC, travessia Cz$ -> NCz$
    [InlineData(1994, 1, 1994, 6, 8.5915)] // Exemplo 4: INPC, primeiro semestre de 1994
    public void ComputeRangeFor_INPC_MatchesPublishedExamples(int y1, int m1, int y2, int m2, double fator)
    {
        var pct = CalculadoraCidadaoBCB.ComputeRangeFor<INPC>(new DateTime(y1, m1, 1), new DateTime(y2, m2, 1));

        Assert.Equal((decimal)fator, Math.Round(1 + (pct / 100), 4));
    }

    /// <summary>
    /// IPCA: o repositório guarda as mesmas 2 casas publicadas pelo IBGE e usadas pela
    /// calculadora, então bate ao centavo — inclusive nos 378 meses de 01/1995 a 06/2026.
    /// </summary>
    [Theory]
    [InlineData(2025, 1, 2025, 2, 1014.72)]
    [InlineData(2025, 2, 2025, 3, 1018.77)]
    [InlineData(2025, 1, 2025, 3, 1020.40)]
    [InlineData(2020, 1, 2025, 12, 1391.53)]
    [InlineData(2010, 1, 2026, 1, 2461.48)]
    [InlineData(1995, 1, 2026, 6, 7528.58)]
    public void AdjustValueWith_IPCA_MatchesBcbCalculator(int y1, int m1, int y2, int m2, double expected)
    {
        var actual = CalculadoraCidadaoBCB.AdjustValueWith<IPCA>(new DateTime(y1, m1, 1), new DateTime(y2, m2, 1), 1000M);

        Assert.Equal((decimal)expected, Math.Round(actual, 2));
    }

    /// <summary>
    /// IGP-M: desde 12/04/2021 a calculadora usa a série de números-índice da FGV com 3 casas
    /// (SGS 28655); o repositório guarda os fechamentos mensais com 2 casas, que era a série
    /// anterior do próprio BCB. O desvio é o arredondamento acumulado, abaixo de 0,05%.
    /// </summary>
    [Theory]
    [InlineData(2015, 1, 2019, 12, 1359.90)]
    [InlineData(2020, 1, 2025, 12, 1561.03)]
    [InlineData(1995, 1, 2026, 6, 11389.12)]
    public void AdjustValueWith_IGPM_MatchesBcbCalculatorWithinRounding(int y1, int m1, int y2, int m2, double expected)
    {
        var actual = CalculadoraCidadaoBCB.AdjustValueWith<IGPM>(new DateTime(y1, m1, 1), new DateTime(y2, m2, 1), 1000M);

        var desvio = Math.Abs(actual - (decimal)expected) / (decimal)expected;
        Assert.True(desvio < 0.0005M, $"esperado ~{expected}, obtido {actual:F2} (desvio {desvio:P4})");
    }

    /// <summary>
    /// Mês único: o BCB aplica o índice daquele mês; Indicator devolve zero.
    /// </summary>
    [Fact]
    public void ComputeRangeFor_SameMonth_AppliesThatMonthRate()
    {
        var mes = new DateTime(2025, 1, 1);

        Assert.Equal(0.16M, CalculadoraCidadaoBCB.ComputeRangeFor<IPCA>(mes, mes));
        Assert.Equal(0M, Indicator.ComputeRangeFor<IPCA>(mes, mes));
    }

    /// <summary>
    /// A convenção do BCB é a do Indicator deslocada um mês, e isso vale para os dois
    /// DataKind: no índice de nível a base passa a ser o mês anterior a dtI.
    /// </summary>
    [Theory]
    [InlineData(2020, 3, 2024, 7)]
    [InlineData(2001, 1, 2001, 1)]
    public void ComputeRangeFor_EqualsIndicatorShiftedOneMonth(int y1, int m1, int y2, int m2)
    {
        var dtI = new DateTime(y1, m1, 1);
        var dtF = new DateTime(y2, m2, 1);

        Assert.Equal(Indicator.ComputeRangeFor<IPCA>(dtI.AddMonths(-1), dtF),
                     CalculadoraCidadaoBCB.ComputeRangeFor<IPCA>(dtI, dtF), 10);

        Assert.Equal(Indicator.ComputeRangeFor<TJus_SP>(dtI.AddMonths(-1), dtF),
                     CalculadoraCidadaoBCB.ComputeRangeFor<TJus_SP>(dtI, dtF), 10);
    }

    /// <summary>
    /// Intervalos adjacentes (sem sobrepor o mês de virada) têm que compor exatamente.
    /// </summary>
    [Fact]
    public void AdjustValueWith_IsChainConsistent()
    {
        var inicio = new DateTime(2020, 1, 1);
        var corte = new DateTime(2022, 6, 1);
        var fim = new DateTime(2025, 12, 1);

        var direto = CalculadoraCidadaoBCB.AdjustValueWith<IPCA>(inicio, fim, 1000M);
        var emEtapas = CalculadoraCidadaoBCB.AdjustValueWith<IPCA>(corte.AddMonths(1), fim,
                       CalculadoraCidadaoBCB.AdjustValueWith<IPCA>(inicio, corte, 1000M));

        Assert.Equal(direto, emEtapas, 8);
    }
}
