using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;

namespace CreditWorks.Domain.Services;

public sealed class CategoryRangeValidator
{
    public void Validate(
        IReadOnlyCollection<VehicleCategory> categories)
    {
        if (categories is null || categories.Count == 0)
            throw new DomainValidationException(
                "At least one category is required.");

        var ordered = categories
            .OrderBy(c => c.Range.MinWeight)
            .ToList();

        // All positive weights must be covered.
        if (ordered[0].Range.MinWeight != 0m)
            throw new DomainValidationException(
                "Category ranges must start at 0 kg.");

        for (int i = 0; i < ordered.Count - 1; i++)
        {
            var current = ordered[i];
            var next = ordered[i + 1];

            if (current.Range.MaxWeight is null)
                throw new DomainValidationException(
                    "Only the final category can have no upper limit.");

            if (current.Range.MaxWeight.Value <
                next.Range.MinWeight)
                throw new DomainValidationException(
                    "Category ranges cannot contain gaps.");

            if (current.Range.MaxWeight.Value >
                next.Range.MinWeight)
                throw new DomainValidationException(
                    "Category ranges cannot overlap.");
        }

        if (ordered[^1].Range.MaxWeight is not null)
            throw new DomainValidationException(
                "The final category must have no upper limit.");
    }
}