
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;

namespace CreditWorks.Domain.Tests.Services;

public class CategoryResolverTests
{
    private readonly CategoryResolver _resolver = new();

    private static VehicleCategory[] CreateCategories()
    {
        return
        [
            new VehicleCategory(
                "Light", "light",
                new WeightRange(0m, 500m)),

            new VehicleCategory(
                "Medium", "medium",
                new WeightRange(500m, 2500m)),

            new VehicleCategory(
                "Heavy", "heavy",
                new WeightRange(2500m, null))
        ];
    }

    [Theory]
    [InlineData(499.99, "Light")]
    [InlineData(500.00, "Medium")]
    [InlineData(1850.75, "Medium")]
    [InlineData(2499.99, "Medium")]
    [InlineData(2500.00, "Heavy")]
    [InlineData(100000.00, "Heavy")]
    public void Resolve_ValidWeight_ReturnsCorrectCategory(
        decimal weight,
        string expected)
    {
        var result = _resolver.Resolve(
            weight,
            CreateCategories());

        Assert.Equal(expected, result.Name);
    }

    [Fact]
    public void Resolve_NegativeWeight_ThrowsException()
    {
        Assert.Throws<DomainValidationException>(
            () => _resolver.Resolve(
                -10m,
                CreateCategories()));
    }

    [Fact]
    public void Resolve_UpdatedRanges_ReturnsNewCategory()
    {
        var original = CreateCategories();

        var originalResult = _resolver.Resolve(
            2200m, original);

        Assert.Equal("Medium", originalResult.Name);

        var updated = new[]
        {
            new VehicleCategory(
                "Light", "light",
                new WeightRange(0m, 500m)),

            new VehicleCategory(
                "Medium", "medium",
                new WeightRange(500m, 2000m)),

            new VehicleCategory(
                "Heavy", "heavy",
                new WeightRange(2000m, null))
        };

        var updatedResult = _resolver.Resolve(
            2200m, updated);

        Assert.Equal("Heavy", updatedResult.Name);
    }
}
