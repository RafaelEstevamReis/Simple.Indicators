namespace Simple.Indicators;

public enum DataKind
{
    Absolute,
    Relative
}
public interface ITable
{
    DataKind Kind { get; }
    int Data_StartYear { get; }
    decimal[][] Data { get; }
}
