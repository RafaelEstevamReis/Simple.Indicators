namespace Simple.Indicators;

using System;

public static class Indicator
{
    public static decimal GetValueFor<T>(DateTime dtRef)
        where T : ITable, new()
    {
        var ind = new T();

        if (ind.Data_StartYear > dtRef.Year) throw new IndexOutOfRangeException($"Data for `{ind.GetType().Name}` available after {ind.Data_StartYear}");

        var ixs = Helpers.GetValueIndexes(ind.Data_StartYear, ind.Data, dtRef);
        if (!Helpers.IsValidIndexes(ixs.Item1, ixs.Item2, ind.Data))
        {
            var maxDate = GetMaxDateFor<T>();
            throw new IndexOutOfRangeException($"Unavailable data for {dtRef:yyyy-MM}, Max date: `{maxDate:yyyy-MM}`");
        }

        return ind.Data[ixs.Item1][ixs.Item2];
    }

    public static DateTime GetMinDateFor<T>()
        where T : ITable, new()
    {
        var ind = new T();
        return new DateTime(ind.Data_StartYear, 1, 1);
    }

    public static DateTime GetMaxDateFor<T>()
        where T : ITable, new()
    {
        var ind = new T();

        var maxYear = ind.Data_StartYear + ind.Data.Length - 1;
        var maxMo = ind.Data[ind.Data.Length - 1].Length;

        return new DateTime(maxYear, maxMo, 1);
    }
}
