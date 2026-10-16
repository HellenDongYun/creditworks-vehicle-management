using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;

namespace CreditWorks.Domain.Services;

public sealed class CategoryResolver
{
    private readonly CategoryRangeValidator _validator = new();

    public VehicleCategory Resolve(
        decimal weight,
        IReadOnlyCollection<VehicleCategory> categories)
    {
        if (weight <= 0)
        {
            throw new DomainValidationException(
                "Vehicle weight must be positive.");
        }

        if (decimal.Round(weight, 2) != weight)
        {
            throw new DomainValidationException(
                "Vehicle weight supports at most two decimal places.");
        }

        if (categories is null || categories.Count == 0)
        {
            throw new DomainValidationException(
                "Vehicle categories are not configured.");
        }

        // Validate the complete category configuration
        _validator.Validate(categories);

        var matches = categories
            .Where(c => c.ContainsWeight(weight))
            .Take(2)
            .ToList();

        if (matches.Count == 0)
        {
            throw new DomainValidationException(
                $"No vehicle category matches weight {weight}.");
        }

        if (matches.Count > 1)
        {
            throw new DomainValidationException(
                "Multiple categories match the same vehicle weight.");
        }

        return matches[0];
    }
}