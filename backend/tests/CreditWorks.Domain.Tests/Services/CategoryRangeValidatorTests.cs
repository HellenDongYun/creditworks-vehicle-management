
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;
using Xunit;

namespace CreditWorks.Domain.Tests.Services;

public sealed class CategoryRangeValidatorTests
{
    private readonly CategoryRangeValidator _validator = new();

    private static VehicleCategory Category(
        string name,
        decimal min,
        decimal? max)
    {
        return new VehicleCategory(
            name,
            "car",
            new WeightRange(min, max));
    }

    [Fact]
    public void Validate_ValidCategories_Succeeds()
    {
        var categories = new[]
        {
            Category("Light", 0m, 500m),
            Category("Medium", 500m, 2500m),
            Category("Heavy", 2500m, null)
        };

        _validator.Validate(categories);
    }

    [Fact]
    public void Validate_UnorderedCategories_Succeeds()
    {
        var categories = new[]
        {
            Category("Heavy", 2500m, null),
            Category("Light", 0m, 500m),
            Category("Medium", 500m, 2500m)
        };

        _validator.Validate(categories);
    }

    [Fact]
    public void Validate_EmptyCategories_Throws()
    {
        var categories = Array.Empty<VehicleCategory>();

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_FirstCategoryDoesNotStartAtZero_Throws()
    {
        var categories = new[]
        {
            Category("Light", 100m, 500m),
            Category("Medium", 500m, null)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_GapBetweenCategories_Throws()
    {
        var categories = new[]
        {
            Category("Light", 0m, 500m),
            Category("Medium", 600m, 2500m),
            Category("Heavy", 2500m, null)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_OverlapBetweenCategories_Throws()
    {
        var categories = new[]
        {
            Category("Light", 0m, 600m),
            Category("Medium", 500m, 2500m),
            Category("Heavy", 2500m, null)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_NonFinalCategoryWithoutUpperLimit_Throws()
    {
        var categories = new[]
        {
            Category("Light", 0m, null),
            Category("Heavy", 2500m, null)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_FinalCategoryWithUpperLimit_Throws()
    {
        var categories = new[]
        {
            Category("Light", 0m, 500m),
            Category("Medium", 500m, 2500m)
        };

        Assert.Throws<DomainValidationException>(
            () => _validator.Validate(categories));
    }

    [Fact]
    public void Validate_SingleUnlimitedCategory_Succeeds()
    {
        var categories = new[]
        {
            Category("All Vehicles", 0m, null)
        };

        _validator.Validate(categories);
    }

    [Fact]
    public void Validate_BoundaryWeightMatchesExactlyOneCategory()
    {
        var categories = new[]
        {
            Category("Light", 0m, 500m),
            Category("Medium", 500m, 2500m),
            Category("Heavy", 2500m, null)
        };

        _validator.Validate(categories);

        Assert.Equal(
            "Light",
            Assert.Single(categories.Where(c =>
                c.ContainsWeight(499.99m))).Name);

        Assert.Equal(
            "Medium",
            Assert.Single(categories.Where(c =>
                c.ContainsWeight(500.00m))).Name);

        Assert.Equal(
            "Medium",
            Assert.Single(categories.Where(c =>
                c.ContainsWeight(2499.99m))).Name);

        Assert.Equal(
            "Heavy",
            Assert.Single(categories.Where(c =>
                c.ContainsWeight(2500.00m))).Name);
    }
}
