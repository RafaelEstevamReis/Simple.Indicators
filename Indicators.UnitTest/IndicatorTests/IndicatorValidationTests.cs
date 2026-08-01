namespace Simple.Indicators.UnitTest.IndicatorTests;

using Simple.Indicators.BR;
using System;

public class IndicatorValidationTests
{
    [Fact]
    public void ComputeRangeFor_InvertedDatesSameYear_Throws()
    {
        Assert.Throws<ArgumentException>(()
            => Indicator.ComputeRangeFor<IPCA>(new DateTime(2012, 6, 1), new DateTime(2012, 2, 1)));
    }

    [Fact]
    public void ComputeRangeFor_InvertedDatesAcrossYears_Throws()
    {
        Assert.Throws<ArgumentException>(()
            => Indicator.ComputeRangeFor<IPCA>(new DateTime(2013, 1, 1), new DateTime(2012, 1, 1)));
    }

    [Fact]
    public void ComputeRangeFor_SameMonthBeforeData_Throws()
    {
        Assert.Throws<IndexOutOfRangeException>(()
            => Indicator.ComputeRangeFor<IPCA>(new DateTime(1500, 1, 1), new DateTime(1500, 1, 1)));
    }

    [Fact]
    public void ComputeRangeFor_SameMonthAfterData_Throws()
    {
        Assert.Throws<IndexOutOfRangeException>(()
            => Indicator.ComputeRangeFor<IPCA>(new DateTime(2400, 5, 1), new DateTime(2400, 5, 1)));
    }

    [Fact]
    public void ComputeRangeFor_SameMonthWithData_ReturnsZero()
    {
        var actual = Indicator.ComputeRangeFor<IPCA>(new DateTime(2025, 5, 1), new DateTime(2025, 5, 1));
        Assert.Equal(0M, actual);
    }

    [Fact]
    public void ComputeRangeFor_AbsoluteWithZeroBase_Throws()
    {
        Assert.Throws<InvalidOperationException>(()
            => Indicator.ComputeRangeFor<FakeDataIndicator_ZeroBase>(new DateTime(2011, 1, 1), new DateTime(2011, 3, 1)));
    }

    [Fact]
    public void AdjustValueWith_InvertedDates_Throws()
    {
        Assert.Throws<ArgumentException>(()
            => Indicator.AdjustValueWith<IPCA>(new DateTime(2013, 1, 1), new DateTime(2012, 1, 1), 1000M));
    }

    [Fact]
    public void GetMaxDateFor_EmptyLastYear_ReturnsLastYearWithData()
    {
        var actual = Indicator.GetMaxDateFor<FakeDataIndicator_EmptyLastYear>();
        Assert.Equal(2011, actual.Year);
        Assert.Equal(3, actual.Month);
    }

    [Fact]
    public void GetMaxDateFor_NoData_Throws()
    {
        Assert.Throws<InvalidOperationException>(()
            => Indicator.GetMaxDateFor<FakeDataIndicator_NoData>());
    }

    [Fact]
    public void GetValueFor_UnavailableDateOnEmptyLastYear_ThrowsIndexOutOfRange()
    {
        // O caminho de erro monta a mensagem com a data máxima e não pode lançar outra exceção
        var ex = Assert.Throws<IndexOutOfRangeException>(()
            => Indicator.GetValueFor<FakeDataIndicator_EmptyLastYear>(new DateTime(2012, 1, 1)));

        Assert.Contains("2011-03", ex.Message);
    }

    [Fact]
    public void GetValueFor_UnavailableDateOnEmptyTable_ThrowsIndexOutOfRange()
    {
        Assert.Throws<IndexOutOfRangeException>(()
            => Indicator.GetValueFor<FakeDataIndicator_NoData>(new DateTime(2011, 1, 1)));
    }
}
