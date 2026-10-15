
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;

namespace CreditWorks.Domain.Tests.Services;

public class CategoryRangeValidatorTests
{
    private readonly CategoryRangeValidator _validator = new();

    private static VehicleCategory Create(
        string name,
        decimal min,
        decimal? max)
    {
        return new VehicleCategory(
            name,
            "icon",
            new WeightRange(min, max));
    }

    [Fact]
    public void Validate_ValidRanges_DoesNotThrow()
    {
        var categories = new[]
        {
            Create("Light", 0m, 500m),
            Create("Medium", 500m, 2500m),
            Create("Heavy", 2500m, null)
        };

        var exception = Record.Exception(
            () => _validator.Validate(categories));

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_Gap_ThrowsException()
    {
        var categories = new[]
        {
            Create("Light", 0m, 500m),
            Create("Medium", 600m, 2500m),
            Create("Heavy", 2500m, null)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_Overlap_ThrowsException()
    {
        var categories = new[]
        {
            Create("Light", 0m, 600m),
            Create("Medium", 500m, 2500m),
            Create("Heavy", 2500m, null)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_MissingInfiniteRange_ThrowsException()
    {
        var categories = new[]
        {
            Create("Light", 0m, 500m),
            Create("Medium", 500m, 2500m)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_EmptyCategories_ThrowsException()
    {
        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(
                Array.Empty<VehicleCategory>()));
    }
}
