namespace Simple.Indicators.UnitTest.IndicatorTests;

using Simple.Indicators.BR;
using System;

public class IndicatorGenericTests
{
    [Fact]
    public void TestFakeDataValues_GetIndicatorValue()
    {
        var actual = Indicator.GetValueFor<FakeDataIndicator_Relative>(new DateTime(2012, 3, 1));
        Assert.Equal(3.2M, actual);
    }
    [Fact]
    public void TestFakeDataValues_GetMinDate()
    {
        var actual = Indicator.GetMinDateFor<FakeDataIndicator_Relative>();
        Assert.Equal(2011, actual.Year);
        Assert.Equal(1, actual.Month);
    }
    [Fact]
    public void TestFakeDataValues_GetMaxDate()
    {
        var actual = Indicator.GetMaxDateFor<FakeDataIndicator_Relative>();
        Assert.Equal(2013, actual.Year);
        Assert.Equal(4, actual.Month);
    }

    [Theory]
    [InlineData(2012, 1, 1, 0)] // SameMonth
    [InlineData(2012, 3, 3, 0)] // SameMonth
    [InlineData(2012, 8, 8, 0)] // SameMonth
    [InlineData(2012, 1, 2, 2.2)] // NextMonth - Exact value
    [InlineData(2012, 3, 4, 4.2)] // NextMonth - Exact value
    [InlineData(2012, 8, 9, 9.2)] // NextMonth - Exact value
    [InlineData(2012, 1, 12, 79.2)] // 12mo
    public void TestFakeDataValues_ComputeRangeFor_Relative(int year, int month1, int month2, decimal result)
    {
        var actual = Indicator.ComputeRangeFor<FakeDataIndicator_Relative>(new DateTime(year, month1, 1), new DateTime(year, month2, 1));
        Assert.Equal(result, actual);
    }

    [Theory]
    [InlineData(2012, 1, 1, 0)] // SameMonth
    [InlineData(2012, 3, 3, 0)] // SameMonth
    [InlineData(2012, 8, 8, 0)] // SameMonth
    [InlineData(2012, 1, 2, 83.33)] // NextMonth - Exact value
    [InlineData(2012, 3, 4, 31.25)] // NextMonth - Exact value
    [InlineData(2012, 8, 9, 12.20)] // NextMonth - Exact value
    [InlineData(2012, 1, 12, 916.67)] // 12mo
    public void TestFakeDataValues_ComputeRangeFor_Absolute(int year, int month1, int month2, decimal result)
    {
        var actual = Indicator.ComputeRangeFor<FakeDataIndicator_Absolute>(new DateTime(year, month1, 1), new DateTime(year, month2, 1));
        Assert.Equal(result, actual, 2);
    }
}
