namespace Simple.Indicators.UnitTest.HelperTests;

using System;

public class IsValidRangeTests
{
    [Theory]
    [InlineData(2012, 1, 2012, 1)] // Mesmo mês
    [InlineData(2012, 1, 2012, 2)] // Mês seguinte
    [InlineData(2012, 12, 2013, 1)] // Ano seguinte
    [InlineData(1980, 6, 2026, 3)] // Intervalo longo
    public void IsValidRange_OrderedDates_ReturnsTrue(int y1, int m1, int y2, int m2)
    {
        Assert.True(DataHelpers.IsValidRange(new DateTime(y1, m1, 1), new DateTime(y2, m2, 1)));
    }

    [Theory]
    [InlineData(2012, 2, 2012, 1)] // Mês anterior
    [InlineData(2013, 1, 2012, 12)] // Ano anterior
    [InlineData(2013, 1, 2012, 1)] // Mesmo mês, ano anterior
    public void IsValidRange_InvertedDates_ReturnsFalse(int y1, int m1, int y2, int m2)
    {
        Assert.False(DataHelpers.IsValidRange(new DateTime(y1, m1, 1), new DateTime(y2, m2, 1)));
    }

    [Fact]
    public void IsValidRange_SameMonthDifferentDays_IgnoresDay()
    {
        Assert.True(DataHelpers.IsValidRange(new DateTime(2012, 3, 31), new DateTime(2012, 3, 1)));
    }
}
