
using CreditWorks.Domain.Exceptions;

namespace CreditWorks.Domain.Entities;

public sealed class Vehicle
{
    public int Id { get; private set; }

    public string OwnerName { get; private set; } = null!;

    public int ManufacturerId { get; private set; }

    public Manufacturer Manufacturer { get; private set; } = null!;

    public int YearOfManufacture { get; private set; }

    public decimal Weight { get; private set; }

    private Vehicle()
    {
        // Required by EF Core
    }

    public Vehicle(
        string ownerName,
        int manufacturerId,
        int yearOfManufacture,
        decimal weight)
    {
        ValidateOwnerName(ownerName);
        ValidateManufacturer(manufacturerId);
        ValidateYear(yearOfManufacture);
        ValidateWeight(weight);

        OwnerName = ownerName.Trim();
        ManufacturerId = manufacturerId;
        YearOfManufacture = yearOfManufacture;
        Weight = weight;
    }

    private static void ValidateOwnerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException(
                "Owner name is required.");

        if (name.Trim().Length > 150)
            throw new DomainValidationException(
                "Owner name cannot exceed 150 characters.");
    }

    private static void ValidateManufacturer(int manufacturerId)
    {
        if (manufacturerId <= 0)
            throw new DomainValidationException(
                "A valid manufacturer is required.");
    }

    private static void ValidateYear(int year)
    {
        if (year < 1886 || year > DateTime.UtcNow.Year + 1)
            throw new DomainValidationException(
                "Year of manufacture is invalid.");
    }

    private static void ValidateWeight(decimal weight)
    {
        if (weight <= 0)
            throw new DomainValidationException(
                "Vehicle weight must be positive.");

        if (decimal.Round(weight, 2) != weight)
            throw new DomainValidationException(
                "Vehicle weight supports at most two decimal places.");

        if (weight >= 10000000000000000m)
            throw new DomainValidationException(
                "Vehicle weight exceeds the supported range.");
    }
}
