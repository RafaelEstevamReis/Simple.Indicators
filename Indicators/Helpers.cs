namespace Indicators;

using System;

public class Helpers
{
    public static (int, int) GetValueIndexes(int startYear, decimal[][] table, DateTime dtRef)
    {
        var ixYear = dtRef.Year - startYear;
        var ixMonth = dtRef.Month - 1;
        return (ixYear, ixMonth);
    }
    public static bool IsValidIndexes(int ixYear, int ixMonth, decimal[][] table)
    {
        if (ixYear < 0) return false;
        if (ixYear >= table.Length) return false;

        if (ixMonth < 0) return false;
        if (ixMonth >= table[ixYear].Length) return false;

        return true;
    }
}
