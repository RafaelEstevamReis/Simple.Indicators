namespace Simple.Indicators.UnitTest.IndicatorTests;

using System;

public class IndicatorGenericTests
{
    [Fact]
    public void TestFakeDataValues_GetIndicatorValue()
    {
        var actual = Indicator.GetValueFor<FakeDataIndicator>(new DateTime(2012, 3, 1));
        Assert.Equal(3.2M, actual);
    }
    [Fact]
    public void TestFakeDataValues_GetMinDate()
    {
        var actual = Indicator.GetMinDateFor<FakeDataIndicator>();
        Assert.Equal(2011, actual.Year);
        Assert.Equal(1, actual.Month);
    }
    [Fact]
    public void TestFakeDataValues_GetMaxDate()
    {
        var actual = Indicator.GetMaxDateFor<FakeDataIndicator>();
        Assert.Equal(2013, actual.Year);
        Assert.Equal(4, actual.Month);
    }


}
