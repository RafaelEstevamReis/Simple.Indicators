namespace Simple.Indicators.API.Models;

public class TableInfo
{
    public int FirstYear { get; init; }
    public int AvailableDataPoints { get; init; }
    public string Kind { get; init; } = default!;
}
