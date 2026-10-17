
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        CreditWorksDbContext context,
        bool seedDemoVehicles = false)
    {
        // Seed manufacturers
        if (!await context.Manufacturers.AnyAsync())
        {
            context.Manufacturers.AddRange(
                new Manufacturer("Mazda"),
                new Manufacturer("Mercedes"),
                new Manufacturer("Honda"),
                new Manufacturer("Ferrari"),
                new Manufacturer("Toyota"));
        }

        // Seed vehicle categories
        if (!await context.VehicleCategories.AnyAsync())
        {
            var categories = new[]
            {
                new VehicleCategory(
                    "Light", "car-front",
                    new WeightRange(0m, 500m)),

                new VehicleCategory(
                    "Medium", "car",
                    new WeightRange(500m, 2500m)),

                new VehicleCategory(
                    "Heavy", "truck",
                    new WeightRange(2500m, null))
            };

            new CategoryRangeValidator().Validate(categories);
            context.VehicleCategories.AddRange(categories);
        }

        // Persist manufacturers and categories first so
        // database-generated manufacturer IDs are available.
        await context.SaveChangesAsync();

        // Demo vehicles are optional and intended only
        // for the Development environment.
        if (!seedDemoVehicles ||
            await context.Vehicles.AnyAsync())
        {
            return;
        }

        var manufacturers = await context.Manufacturers
            .AsNoTracking()
            .ToDictionaryAsync(m => m.Name, m => m.Id);

        int ManufacturerId(string name)
        {
            if (!manufacturers.TryGetValue(name, out var id))
            {
                throw new InvalidOperationException(
                    $"Required demo manufacturer '{name}' was not found.");
            }

            return id;
        }

        context.Vehicles.AddRange(
            new Vehicle(
                "Alice Johnson",
                ManufacturerId("Mazda"),
                2020,
                450m),

            new Vehicle(
                "Bob Smith",
                ManufacturerId("Toyota"),
                2022,
                500m),

            new Vehicle(
                "Charlie Brown",
                ManufacturerId("Honda"),
                2019,
                980m),

            new Vehicle(
                "Diana Wilson",
                ManufacturerId("Mercedes"),
                2023,
                1450m),

            new Vehicle(
                "Ethan Davis",
                ManufacturerId("Ferrari"),
                2021,
                1680m),

            new Vehicle(
                "Fiona Taylor",
                ManufacturerId("Toyota"),
                2018,
                2499.99m),

            new Vehicle(
                "George Miller",
                ManufacturerId("Mercedes"),
                2024,
                2500m),

            new Vehicle(
                "Hannah Lee",
                ManufacturerId("Mazda"),
                2020,
                3200m)
        );

        await context.SaveChangesAsync();
    }
}
