namespace Simple.Indicators;

using System;
using System.Linq;

public static class Indicator
{
    /// <summary>
    /// Obtém valor para a data
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Data sem dados disponiveis</exception>
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

    /// <summary>
    /// Retorna a data mais antiga que há dados
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static DateTime GetMinDateFor<T>()
        where T : ITable, new()
    {
        var ind = new T();
        return new DateTime(ind.Data_StartYear, 1, 1);
    }
    /// <summary>
    /// Retorna a data mais recente que há dados
    /// </summary>
    public static DateTime GetMaxDateFor<T>()
        where T : ITable, new()
    {
        var ind = new T();

        var maxYear = ind.Data_StartYear + ind.Data.Length - 1;
        var maxMo = ind.Data[ind.Data.Length - 1].Length;

        return new DateTime(maxYear, maxMo, 1);
    }

    /// <summary>
    /// Calcula a variação entre meses (não considera dias)
    /// O mês inicial é excludente para dados relativos
    /// Mesmo mês: =0
    /// </summary>
    /// <exception cref="InvalidOperationException">DataKind inválido</exception>
    /// <exception cref="IndexOutOfRangeException">Data sem dados disponiveis</exception>
    public static decimal ComputeRangeFor<T>(DateTime dtI, DateTime dtF)
        where T : ITable, new()
    {
        // Regras gerais:
        // 1. Mesmo mês, retorna 0
        if (dtI.Year == dtF.Year && dtI.Month == dtF.Month) return 0M;
        // 2. Do mês [A] para o mês seguinte, taxa do mês A

        var ind = new T();
        decimal[] values = Helpers.GetValueSpan(ind, dtI, dtF);

        if (ind.Kind == DataKind.Absolute)
        {
            decimal initialValue = values[0];
            decimal finalValue = values[values.Length - 1];

            return 100 * ((finalValue / initialValue) - 1);
        }
        else if (ind.Kind == DataKind.Relative)
        {
            // Descarta o último
            return values.Skip(1)
                         .Sum();
        }
        else throw new InvalidOperationException("Unsupported DataKind.");
    }
}
