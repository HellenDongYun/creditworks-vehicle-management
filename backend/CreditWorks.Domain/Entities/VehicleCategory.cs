using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.ValueObjects;

namespace CreditWorks.Domain.Entities;

public sealed class VehicleCategory
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Icon { get; private set; } = null!;

    public WeightRange Range { get; private set; } = null!;

    private VehicleCategory()
    {
        // Required by EF Core
    }

    public VehicleCategory(
        string name,
        string icon,
        WeightRange range)
    {
        Validate(name, icon, range);

        Name = name.Trim();
        Icon = icon.Trim();
        Range = range;
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
        Validate(name, icon, range);

        Name = name.Trim();
        Icon = icon.Trim();
        Range = range;
    }

    private static void Validate(
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

        if (range is null)
            throw new DomainValidationException(
                "Category range is required.");

        if (name.Trim().Length > 100)
            throw new DomainValidationException(
                "Category name cannot exceed 100 characters.");

        if (icon.Trim().Length > 100)
            throw new DomainValidationException(
                "Category icon cannot exceed 100 characters.");
    }
}