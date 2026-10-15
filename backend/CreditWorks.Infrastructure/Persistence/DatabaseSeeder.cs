using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        CreditWorksDbContext context)
    {
        if (!await context.Manufacturers.AnyAsync())
        {
            context.Manufacturers.AddRange(
                new Manufacturer("Mazda"),
                new Manufacturer("Mercedes"),
                new Manufacturer("Honda"),
                new Manufacturer("Ferrari"),
                new Manufacturer("Toyota"));
        }

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

        await context.SaveChangesAsync();
    }
}