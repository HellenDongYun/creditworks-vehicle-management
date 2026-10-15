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
            throw new DomainValidationException(
                "Vehicle weight must be positive.");

        if (decimal.Round(weight, 2) != weight)
            throw new DomainValidationException(
                "Vehicle weight supports at most two decimal places.");

        _validator.Validate(categories);

        return categories.Single(c => c.ContainsWeight(weight));
    }
}