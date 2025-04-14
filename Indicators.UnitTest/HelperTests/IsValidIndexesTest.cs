namespace Simple.Indicators.UnitTest;

using Xunit;

public class IsValidIndexesTest
{
    [Fact]
    public void IsValidIndexes_ValidIndexes_ReturnsTrue()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m, 2.0m, 3.0m],
            [4.0m, 5.0m]
        ];
        int ixYear = 1;
        int ixMonth = 1;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidIndexes_ZeroIndexes_ReturnsTrue()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m],
            [2.0m]
        ];
        int ixYear = 0;
        int ixMonth = 0;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidIndexes_NegativeYear_ReturnsFalse()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m]
        ];
        int ixYear = -1;
        int ixMonth = 0;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidIndexes_NegativeMonth_ReturnsFalse()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m]
        ];
        int ixYear = 0;
        int ixMonth = -1;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidIndexes_YearExceedsTableLength_ReturnsFalse()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m]
        ];
        int ixYear = 1;
        int ixMonth = 0;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidIndexes_MonthExceedsRowLength_ReturnsFalse()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m]
        ];
        int ixYear = 0;
        int ixMonth = 1;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidIndexes_MaxValidIndexes_ReturnsTrue()
    {
        // Arrange
        decimal[][] table =
        [
            [1.0m, 2.0m],
            [3.0m, 4.0m]
        ];
        int ixYear = 1;
        int ixMonth = 1;

        // Act
        bool result = Helpers.IsValidIndexes(ixYear, ixMonth, table);

        // Assert
        Assert.True(result);
    }
}
