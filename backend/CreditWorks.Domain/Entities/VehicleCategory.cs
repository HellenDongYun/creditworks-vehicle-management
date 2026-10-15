using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.ValueObjects;

namespace CreditWorks.Domain.Entities;

public sealed class VehicleCategory
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public WeightRange Range { get; private set; }

    public VehicleCategory(
        string name,
        string icon,
        WeightRange range)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException(
                "Category name is required.");

        if (string.IsNullOrWhiteSpace(icon))
            throw new DomainValidationException(
                "Category icon is required.");

        Name = name.Trim();
        Icon = icon.Trim();
        Range = range ?? throw new DomainValidationException(
            "Category range is required.");
    }

    public bool ContainsWeight(decimal weight)
    {
        return Range.Contains(weight);
    }

    public void Update(
        string name,
        string icon,
        WeightRange range)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException(
                "Category name is required.");

        if (string.IsNullOrWhiteSpace(icon))
            throw new DomainValidationException(
                "Category icon is required.");

        Name = name.Trim();
        Icon = icon.Trim();
        Range = range ?? throw new DomainValidationException(
            "Category range is required.");
    }
}