using CreditWorks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.Infrastructure.Persistence;

public sealed class CreditWorksDbContext : DbContext
{
    public CreditWorksDbContext(
        DbContextOptions<CreditWorksDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<Manufacturer> Manufacturers =>
        Set<Manufacturer>();

    public DbSet<VehicleCategory> VehicleCategories =>
        Set<VehicleCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CreditWorksDbContext).Assembly);
    }
}