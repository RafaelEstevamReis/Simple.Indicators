namespace Simple.Indicators.UnitTest.HelperTests;

using System;
using Indicators.UnitTest;

public class GetValueSpanTests
{
    [Fact]
    public void GetValueSpan_FakeData_SameYearRange()
    {
        var ind = new FakeDataIndicator();

        var span = Helpers.GetValueSpan(ind, new DateTime(2012, 2, 1), new DateTime(2012, 5, 1));
        Assert.Equal(4, span.Length);
        Assert.Equal(2.2M, span[0]);
        Assert.Equal(5.2M, span[3]);
    }

    [Fact]
    public void GetValueSpan_FakeData_NextYearRange()
    {
        var ind = new FakeDataIndicator();

        var span = Helpers.GetValueSpan(ind, new DateTime(2012, 2, 1), new DateTime(2013, 4, 1));
        Assert.Equal(15, span.Length);
        Assert.Equal(2.2M, span[0]);
        Assert.Equal(4.3M, span[14]);
    }

    [Fact]
    public void GetValueSpan_FakeData_Range()
    {
        var ind = new FakeDataIndicator();

        var span = Helpers.GetValueSpan(ind, new DateTime(2011, 8, 1), new DateTime(2013, 2, 1));
        Assert.Equal(19, span.Length);
        Assert.Equal(8.1M, span[0]);
        Assert.Equal(2.3M, span[18]);
    }
}
