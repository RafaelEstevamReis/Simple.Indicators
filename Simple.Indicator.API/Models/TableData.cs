namespace Simple.Indicators.API.Models;

public class TableData
{
    public string TableName { get; init; } = default!;
    public int FirstYear { get; init; }
    public string Kind { get; init; } = default!;
    public decimal[] Data { get; init; } = [];
}
