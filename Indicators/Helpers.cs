namespace Simple.Indicators;

using System;
using System.Collections.Generic;
using System.Linq;

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
    public static decimal[] GetValueSpan(ITable table, DateTime dtI, DateTime dtF)
    {
        var ixStart = GetValueIndexes(table.Data_StartYear, table.Data, dtI);
        if (!IsValidIndexes(ixStart.Item1, ixStart.Item2, table.Data))
        {
            throw new IndexOutOfRangeException($"Unavailable data for {dtI:yyyy-MM}`");
        }

        var ixEnd = GetValueIndexes(table.Data_StartYear, table.Data, dtF);
        if (!IsValidIndexes(ixEnd.Item1, ixEnd.Item2, table.Data))
        {
            throw new IndexOutOfRangeException($"Unavailable data for {dtF:yyyy-MM}`");
        }

        // Mesmo ano
        if (ixStart.Item1 == ixEnd.Item1)
        {
            decimal[] arr = new decimal[ixEnd.Item2 - ixStart.Item2 + 1];
            Array.Copy(table.Data[ixStart.Item1], ixStart.Item2, arr, 0, arr.Length);
            return arr;
        }

        List<decimal> lstItems = new List<decimal>();

        // Adiciona ano inicial
        lstItems.AddRange(table.Data[ixStart.Item1].Skip(ixStart.Item2));

        // Adiciona intermediários
        for (int i = ixStart.Item1 + 1; i < ixEnd.Item1; i++)
        {
            lstItems.AddRange(table.Data[i]);
        }

        // Adiciona ano final
        lstItems.AddRange(table.Data[ixEnd.Item1].Take(ixEnd.Item2 + 1)); // Ix to Count

        return lstItems.ToArray();
    }
}
