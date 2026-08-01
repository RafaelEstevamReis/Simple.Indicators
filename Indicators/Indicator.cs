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

        var ixs = DataHelpers.GetValueIndexes(ind.Data_StartYear, ind.Data, dtRef);
        if (!DataHelpers.IsValidIndexes(ixs.Item1, ixs.Item2, ind.Data))
        {
            var maxDate = tryGetMaxDate(ind);
            throw new IndexOutOfRangeException(maxDate.HasValue
                ? $"Unavailable data for {dtRef:yyyy-MM}, Max date: `{maxDate.Value:yyyy-MM}`"
                : $"Unavailable data for {dtRef:yyyy-MM}");
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
    /// <exception cref="InvalidOperationException">Tabela sem nenhum ano válido</exception>
    public static DateTime GetMaxDateFor<T>()
        where T : ITable, new()
    {
        var ind = new T();

        return tryGetMaxDate(ind)
            ?? throw new InvalidOperationException($"Table `{ind.GetType().Name}` has no valid data.");
    }
    /// <summary>
    /// Último mês com dados, ou null caso a tabela não tenha nenhum ano válido.
    /// Ignora anos sem meses ou com mais de 12 meses, que não representam uma data.
    /// </summary>
    private static DateTime? tryGetMaxDate(ITable ind)
    {
        for (int ixYear = ind.Data.Length - 1; ixYear >= 0; ixYear--)
        {
            var months = ind.Data[ixYear].Length;
            if (months < 1 || months > 12) continue;

            return new DateTime(ind.Data_StartYear + ixYear, months, 1);
        }

        return null;
    }

    /// <summary>
    /// Calcula a variação entre meses (não considera dias)
    /// O mês inicial é excludente para dados relativos
    /// Mesmo mês: =0
    /// </summary>
    /// <exception cref="ArgumentException">Data inicial posterior à data final</exception>
    /// <exception cref="InvalidOperationException">DataKind inválido ou valor base zerado</exception>
    /// <exception cref="IndexOutOfRangeException">Data sem dados disponiveis</exception>
    public static decimal ComputeRangeFor<T>(DateTime dtI, DateTime dtF)
        where T : ITable, new()
    {
        // Regras gerais:
        // 1. Mesmo mês, retorna 0
        // 2. Do mês [A] para o mês seguinte, taxa do mês A

        var ind = new T();

        // Valida a ordem das datas e a disponibilidade de ambas
        decimal[] values = DataHelpers.GetValueSpan(ind, dtI, dtF);

        // Mesmo mês: o intervalo tem um único valor
        if (values.Length == 1) return 0M;

        if (ind.Kind == DataKind.Absolute)
        {
            decimal initialValue = values[0];
            decimal finalValue = values[values.Length - 1];

            if (initialValue == 0M) throw new InvalidOperationException($"Table `{ind.GetType().Name}` has a zero base value for {dtI:yyyy-MM}.");

            return 100 * ((finalValue / initialValue) - 1);
        }
        else if (ind.Kind == DataKind.Relative)
        {
            // Taxas mensais capitalizam: o acumulado é o produto, não a soma.
            // Descarta o primeiro: o mês inicial é excludente.
            decimal accum = 1M;
            for (int i = 1; i < values.Length; i++)
            {
                accum *= 1 + (values[i] / 100);
            }

            return (accum - 1) * 100;
        }
        else throw new InvalidOperationException("Unsupported DataKind.");
    }

    /// <summary>
    /// Corrige um valor com o indicador escolhido para as datas
    /// </summary>
    /// <typeparam name="T">Indicador</typeparam>
    /// <param name="dtI">Data Inicial</param>
    /// <param name="dtF">Data final</param>
    /// <param name="value">Valor a ser corrigido</param>
    /// <returns>Valor corrigido no período</returns>
    public static decimal AdjustValueWith<T>(DateTime dtI, DateTime dtF, decimal value)
         where T : ITable, new()
    {
        var variation = ComputeRangeFor<T>(dtI, dtF);
        return (1 + (variation / 100)) * value;
    }
}
