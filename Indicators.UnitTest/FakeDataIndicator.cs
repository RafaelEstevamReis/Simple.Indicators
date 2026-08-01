namespace Simple.Indicators.UnitTest;

using Simple.Indicators;


public class FakeDataIndicator_Relative : ITable
{
    public DataKind Kind => DataKind.Relative;
    public int Data_StartYear { get; } = 2011;
    public decimal[][] Data => _Data;
    public static decimal[][] _Data =
    [
        [ 1.1M, 2.1M, 3.1M, 4.1M, 5.1M, 6.1M, 7.1M, 8.1M, 9.1M, 10.1M, 11.1M, 12.1M], // 2011
        [ 1.2M, 2.2M, 3.2M, 4.2M, 5.2M, 6.2M, 7.2M, 8.2M, 9.2M, 10.2M, 11.2M, 12.2M], // 2012
        [ 1.3M, 2.3M, 3.3M, 4.3M], // 2013
    ];
}
public class FakeDataIndicator_Absolute : ITable
{
    public DataKind Kind => DataKind.Absolute;
    public int Data_StartYear { get; } = 2011;
    public decimal[][] Data => FakeDataIndicator_Relative._Data;
}

/// <summary>Último ano sem meses preenchidos</summary>
public class FakeDataIndicator_EmptyLastYear : ITable
{
    public DataKind Kind => DataKind.Relative;
    public int Data_StartYear { get; } = 2011;
    public decimal[][] Data { get; } =
    [
        [ 1.1M, 2.1M, 3.1M], // 2011
        [], // 2012
    ];
}
/// <summary>Tabela sem nenhum ano</summary>
public class FakeDataIndicator_NoData : ITable
{
    public DataKind Kind => DataKind.Relative;
    public int Data_StartYear { get; } = 2011;
    public decimal[][] Data { get; } = [];
}
/// <summary>Absoluto com valor base zerado</summary>
public class FakeDataIndicator_ZeroBase : ITable
{
    public DataKind Kind => DataKind.Absolute;
    public int Data_StartYear { get; } = 2011;
    public decimal[][] Data { get; } =
    [
        [ 0M, 1M, 2M], // 2011
    ];
}