
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.ValueObjects;

namespace CreditWorks.Domain.Tests.ValueObjects;

public class WeightRangeTests
{
    // Test 1: Light category boundaries
    [Theory]
    [InlineData(0.01, true)]
    [InlineData(100.00, true)]
    [InlineData(499.99, true)]
    [InlineData(500.00, false)]
    [InlineData(0.00, false)]
    [InlineData(-1.00, false)]
    public void Contains_LightRange_ReturnsExpectedResult(
        decimal weight,
        bool expected)
    {
        // Arrange
        var range = new WeightRange(0m, 500m);

        // Act
        var result = range.Contains(weight);

        // Assert
        Assert.Equal(expected, result);
    }

    // Test 2: Medium category boundaries
    [Theory]
    [InlineData(499.99, false)]
    [InlineData(500.00, true)]
    [InlineData(1850.75, true)]
    [InlineData(2499.99, true)]
    [InlineData(2500.00, false)]
    public void Contains_MediumRange_ReturnsExpectedResult(
        decimal weight,
        bool expected)
    {
        var range = new WeightRange(500m, 2500m);

        Assert.Equal(expected, range.Contains(weight));
    }

    // Test 3: Heavy category has no upper limit
    [Theory]
    [InlineData(2499.99, false)]
    [InlineData(2500.00, true)]
    [InlineData(5000.00, true)]
    [InlineData(100000.00, true)]
    public void Contains_HeavyRange_ReturnsExpectedResult(
        decimal weight,
        bool expected)
    {
        var range = new WeightRange(2500m, null);

        Assert.Equal(expected, range.Contains(weight));
    }

    // Test 4: Negative minimum weight is invalid
    [Fact]
    public void Constructor_NegativeMinimum_ThrowsException()
    {
        Assert.Throws<DomainValidationException>(
            () => new WeightRange(-1m, 500m));
    }

    // Test 5: Maximum must be greater than minimum
    [Theory]
    [InlineData(500, 500)]
    [InlineData(600, 500)]
    public void Constructor_InvalidRange_ThrowsException(
        decimal min,
        decimal max)
    {
        Assert.Throws<DomainValidationException>(
            () => new WeightRange(min, max));
    }

    // Test 6: Weight boundaries allow at most 2 decimals
    [Theory]
    [InlineData(0.001, 500)]
    [InlineData(0, 500.001)]
    public void Constructor_MoreThanTwoDecimals_ThrowsException(
        decimal min,
        decimal max)
    {
        Assert.Throws<DomainValidationException>(
            () => new WeightRange(min, max));
    }

    // Test 7: Valid ranges can be created
    [Fact]
    public void Constructor_ValidRange_CreatesObject()
    {
        var range = new WeightRange(500m, 2500m);

        Assert.Equal(500m, range.MinWeight);
        Assert.Equal(2500m, range.MaxWeight);
    }

    // Test 8: Unlimited maximum is supported
    [Fact]
    public void Constructor_NullMaximum_IsAllowed()
    {
        var range = new WeightRange(2500m, null);

        Assert.Equal(2500m, range.MinWeight);
        Assert.Null(range.MaxWeight);
    }
}

