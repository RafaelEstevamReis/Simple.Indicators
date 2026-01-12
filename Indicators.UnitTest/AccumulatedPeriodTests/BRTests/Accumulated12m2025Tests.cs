namespace Simple.Indicators.UnitTest.AccumulatedPeriodTests.BRTests;

using Simple.Indicators.BR;
using System;

public class Accumulated12m2025Tests
{
    static DateTime Start = new DateTime(2025, 1, 1);
    static DateTime End = new DateTime(2025, 12, 31);

    [Fact]
    public void Accumulated12m2025_BR_CDI()
    {
        var table = new CDI();
        var span = Helpers.GetValueSpan(table, Start, End);
        var expected = 14.3328M;

        var numericActual = Helpers.CalculatePercentVariation(span);
        var percentActual = (numericActual - 1) * 100;

        Assert.Equal(expected, percentActual, 4);
    }

    [Fact]
    public void Accumulated12m2025_BR_IGPM()
    {
        var table = new IGPM();
        var span = Helpers.GetValueSpan(table, Start, End);
        var expected = -1.0422M;

        var numericActual = Helpers.CalculatePercentVariation(span);
        var percentActual = (numericActual - 1) * 100;

        Assert.Equal(expected, percentActual, 4);
    }

    [Fact]
    public void Accumulated12m2025_BR_INPC()
    {
        var table = new INPC();
        var span = Helpers.GetValueSpan(table, Start, End);
        var expected = 3.8979M;

        var numericActual = Helpers.CalculatePercentVariation(span);
        var percentActual = (numericActual - 1) * 100;

        Assert.Equal(expected, percentActual, 4);
    }

    [Fact]
    public void Accumulated12m2025_BR_IPCA()
    {
        var table = new IPCA();
        var span = Helpers.GetValueSpan(table, Start, End);
        var expected = 4.2644M;

        var numericActual = Helpers.CalculatePercentVariation(span);
        var percentActual = (numericActual - 1) * 100;

        Assert.Equal(expected, percentActual, 4);
    }

    [Fact]
    public void Accumulated12m2025_BR_Poupanca()
    {
        var table = new Poupanca();
        var span = Helpers.GetValueSpan(table, Start, End);
        var expected = 8.2634M;

        var numericActual = Helpers.CalculatePercentVariation(span);
        var percentActual = (numericActual - 1) * 100;

        Assert.Equal(expected, percentActual, 4);
    }

    [Fact]
    public void Accumulated12m2025_BR_Selic()
    {
        var table = new Selic();
        var span = Helpers.GetValueSpan(table, Start, End);
        var expected = 14.3328M;

        var numericActual = Helpers.CalculatePercentVariation(span);
        var percentActual = (numericActual - 1) * 100;

        Assert.Equal(expected, percentActual, 4);
    }

}
