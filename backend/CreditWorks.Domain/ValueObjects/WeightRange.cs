using CreditWorks.Domain.Exceptions;

namespace CreditWorks.Domain.ValueObjects;

public sealed record WeightRange
{
    public decimal MinWeight { get; }
    public decimal? MaxWeight { get; }

    public WeightRange(decimal minWeight, decimal? maxWeight)
    {
        if (minWeight < 0)
            throw new DomainValidationException(
                "Minimum weight cannot be negative.");

        if (maxWeight.HasValue && maxWeight.Value <= minWeight)
            throw new DomainValidationException(
                "Maximum weight must be greater than minimum weight.");

        if (decimal.Round(minWeight, 2) != minWeight ||
            (maxWeight.HasValue &&
             decimal.Round(maxWeight.Value, 2) != maxWeight.Value))
            throw new DomainValidationException(
                "Weight boundaries support at most two decimal places.");

        MinWeight = minWeight;
        MaxWeight = maxWeight;
    }

    public bool Contains(decimal weight)
    {
        return weight > 0
               && weight >= MinWeight
               && (!MaxWeight.HasValue || weight < MaxWeight.Value);
    }
}