namespace Simple.Indicators.UnitTest.TableTests;

using System;
using System.Linq;

public class TableSanityTests
{
    public static TheoryData<string> AllTableNames()
    {
        var data = new TheoryData<string>();
        foreach (var table in TableHelper.GetAllTables())
        {
            data.Add(table.GetType().FullName!);
        }
        return data;
    }

    [Fact]
    public void AllTables_AreDiscovered()
    {
        Assert.NotEmpty(TableHelper.GetAllTables());
    }

    /// <summary>
    /// Um ano nunca pode ter mais de 12 meses nem ficar vazio.
    /// Mais de 12 indica erro de digitação (ex.: `0,54M` no lugar de `0.54M`, que vira dois itens).
    /// </summary>
    [Theory]
    [MemberData(nameof(AllTableNames))]
    public void Table_EveryYearHasOneToTwelveMonths(string tableName)
    {
        var table = TableHelper.GetTableByName(tableName);
        Assert.NotNull(table);

        var invalid = table!.Data
                            .Select((months, ix) => (Year: table.Data_StartYear + ix, Count: months.Length))
                            .Where(o => o.Count < 1 || o.Count > 12)
                            .Select(o => $"{o.Year}: {o.Count}")
                            .ToArray();

        Assert.True(invalid.Length == 0, $"Anos com quantidade inválida de meses -> {string.Join(", ", invalid)}");
    }

    /// <summary>
    /// Só o último ano pode estar incompleto; um ano curto no meio desloca todos os meses seguintes.
    /// </summary>
    [Theory]
    [MemberData(nameof(AllTableNames))]
    public void Table_OnlyLastYearMayBeIncomplete(string tableName)
    {
        var table = TableHelper.GetTableByName(tableName);
        Assert.NotNull(table);

        var incomplete = table!.Data
                               .Take(table.Data.Length - 1)
                               .Select((months, ix) => (Year: table.Data_StartYear + ix, Count: months.Length))
                               .Where(o => o.Count != 12)
                               .Select(o => $"{o.Year}: {o.Count}")
                               .ToArray();

        Assert.True(incomplete.Length == 0, $"Anos incompletos fora do último -> {string.Join(", ", incomplete)}");
    }

    /// <summary>
    /// A última data precisa ser resolvível: o último ano tem que ter entre 1 e 12 meses.
    /// </summary>
    [Theory]
    [MemberData(nameof(AllTableNames))]
    public void Table_HasResolvableMaxDate(string tableName)
    {
        var table = TableHelper.GetTableByName(tableName);
        Assert.NotNull(table);
        Assert.NotEmpty(table!.Data);

        var months = table.Data[table.Data.Length - 1].Length;
        var maxDate = new DateTime(table.Data_StartYear + table.Data.Length - 1, months, 1);

        Assert.InRange(maxDate, new DateTime(table.Data_StartYear, 1, 1), DateTime.MaxValue);
    }

    /// <summary>
    /// Data precisa ser um array estático compartilhado: um inicializador de instância
    /// realoca a tabela inteira a cada `new T()` feito pelo Indicator.
    /// </summary>
    [Theory]
    [MemberData(nameof(AllTableNames))]
    public void Table_DataIsSharedAcrossInstances(string tableName)
    {
        var type = TableHelper.GetTableByName(tableName)!.GetType();

        var first = (ITable)Activator.CreateInstance(type)!;
        var second = (ITable)Activator.CreateInstance(type)!;

        Assert.Same(first.Data, second.Data);
    }

    /// <summary>
    /// As cadeias de correção monetária (ORTN/OTN/BTN/BTN-TR) são índices de nível que nunca caem.
    /// Uma queda denuncia erro de transcrição: dígito trocado ou vírgula fora de lugar.
    /// Não vale para IBC-Br, M2 e TJus_SP, que são agregados reais e podem recuar.
    /// </summary>
    [Theory]
    [InlineData("Simple.Indicators.BR.hORTN64")]
    [InlineData("Simple.Indicators.BR.hOTN86")]
    [InlineData("Simple.Indicators.BR.hBTN89")]
    [InlineData("Simple.Indicators.BR.BTN_TR")]
    public void Table_MonetaryChainNeverDecreases(string tableName)
    {
        var table = TableHelper.GetTableByName(tableName);
        Assert.NotNull(table);

        var flat = table!.Data
                         .SelectMany((months, ixYear) => months.Select((value, ixMonth)
                             => (Year: table.Data_StartYear + ixYear, Month: ixMonth + 1, Value: value)))
                         .ToArray();

        var drops = flat.Zip(flat.Skip(1), (previous, current) => (previous, current))
                        .Where(o => o.current.Value < o.previous.Value)
                        .Select(o => $"{o.current.Year}-{o.current.Month:00}: {o.previous.Value} -> {o.current.Value}")
                        .ToArray();

        Assert.True(drops.Length == 0, $"Quedas em índice de nível -> {string.Join("; ", drops)}");
    }

    /// <summary>
    /// Pontos que já vieram errados por transcrição, conferidos contra três fontes independentes:
    /// e-Calculos, debit.com.br e maisjuridico (série ORTN / OTN / BTN / BTN-TR, origem BDI/STN-MF).
    /// </summary>
    [Theory]
    [InlineData("Simple.Indicators.BR.hORTN64", 1966, 1, 16.60)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1966, 7, 19.87)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1966, 9, 21.01)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1972, 3, 63.09)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1972, 11, 69.61)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1975, 6, 117.13)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1978, 5, 262.87)]
    [InlineData("Simple.Indicators.BR.hORTN64", 1978, 11, 310.49)]
    [InlineData("Simple.Indicators.BR.hOTN86", 1986, 2, 93.039)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1993, 12, 130632)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1994, 1, 178705)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1994, 2, 252760)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1994, 3, 353511)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1994, 4, 501455)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1994, 5, 731975)]
    [InlineData("Simple.Indicators.BR.hBTN89", 1994, 6, 1071903)]
    public void Table_MatchesOfficialSource(string tableName, int year, int month, double expected)
    {
        var table = TableHelper.GetTableByName(tableName);
        Assert.NotNull(table);

        var actual = table!.Data[year - table.Data_StartYear][month - 1];

        Assert.Equal((decimal)expected, actual);
    }
}
