namespace Simple.Indicators.BR;

using System;

/// <summary>
/// Correção de valores na convenção da Calculadora do Cidadão do Banco Central do Brasil.
/// <para>
/// Metodologia: https://www3.bcb.gov.br/CALCIDADAO/publico/metodologiaCorrigirIndice.do
/// </para>
/// <para>
/// Diferente de <see cref="Indicator"/>, que trata o mês inicial como excludente, o BCB usa
/// os índices da data inicial <b>e</b> da data final: corrigir de 01/2025 até 02/2025 aplica
/// janeiro e fevereiro. Para corrigir um único mês, informe a mesma data nos dois parâmetros.
/// </para>
/// </summary>
public static class CalculadoraCidadaoBCB
{
    /// <summary>
    /// Calcula a variação acumulada entre os meses, extremos inclusive (não considera dias)
    /// <para>
    /// Equivale a <see cref="Indicator.ComputeRangeFor{T}"/> começando um mês antes, e é assim
    /// que os dados absolutos são tratados: a variação de [dtI, dtF] num índice de nível é
    /// nível(dtF) / nível(dtI - 1 mês).
    /// </para>
    /// <remarks>
    /// O fator não depende da moeda vigente, mas o valor corrigido sim: se houve troca de
    /// padrão monetário no período, a conversão é responsabilidade do chamador — mesma regra
    /// documentada pelo BCB.
    /// </remarks>
    /// </summary>
    /// <exception cref="ArgumentException">Data inicial posterior à data final</exception>
    /// <exception cref="InvalidOperationException">DataKind inválido ou valor base zerado</exception>
    /// <exception cref="IndexOutOfRangeException">Data sem dados disponiveis</exception>
    public static decimal ComputeRangeFor<T>(DateTime dtI, DateTime dtF)
        where T : ITable, new()
    {
        var ind = new T();

        if (ind.Kind == DataKind.Relative)
        {
            // Capitaliza todos os meses do intervalo, extremos inclusive
            var values = DataHelpers.GetValueSpan(ind, dtI, dtF);

            return (DataHelpers.CalculatePercentVariation(values) - 1) * 100;
        }

        if (ind.Kind == DataKind.Absolute)
        {
            // Índice de nível: o mês inicial só entra se a base for o mês anterior
            return Indicator.ComputeRangeFor<T>(dtI.AddMonths(-1), dtF);
        }

        throw new InvalidOperationException("Unsupported DataKind.");
    }

    /// <summary>
    /// Corrige um valor com o indicador escolhido para as datas, extremos inclusive
    /// </summary>
    /// <typeparam name="T">Indicador</typeparam>
    /// <param name="dtI">Data inicial, inclusive</param>
    /// <param name="dtF">Data final, inclusive</param>
    /// <param name="value">Valor a ser corrigido</param>
    /// <returns>Valor corrigido no período</returns>
    public static decimal AdjustValueWith<T>(DateTime dtI, DateTime dtF, decimal value)
        where T : ITable, new()
    {
        var variation = ComputeRangeFor<T>(dtI, dtF);
        return (1 + (variation / 100)) * value;
    }
}
