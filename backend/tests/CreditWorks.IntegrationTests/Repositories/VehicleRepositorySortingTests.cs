
using CreditWorks.Domain.Entities;
using CreditWorks.Infrastructure.Persistence;
using CreditWorks.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace CreditWorks.IntegrationTests.Repositories;

public sealed class VehicleRepositorySortingTests
    : IAsyncLifetime
{
    
    private readonly MsSqlContainer _container =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
            .Build();


    private CreditWorksDbContext _context = null!;

    public async Task InitializeAsync()
    {
        // Start an isolated SQL Server container.
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<CreditWorksDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        _context = new CreditWorksDbContext(options);

        // Create tables using the real EF Core model.
        await _context.Database.EnsureCreatedAsync();

        // Seed test manufacturers.
        var mazda = new Manufacturer("Mazda");
        var honda = new Manufacturer("Honda");
        var toyota = new Manufacturer("Toyota");

        _context.Manufacturers.AddRange(mazda, honda, toyota);

        await _context.SaveChangesAsync();

        // Seed test vehicles.
        _context.Vehicles.AddRange(
            new Vehicle("John Smith", mazda.Id, 2022, 1850.75m),
            new Vehicle("John Smith", mazda.Id, 2020, 1500.55m),
            new Vehicle("John Smith", honda.Id, 2020, 1500.55m),
            new Vehicle("Mary Johnson", toyota.Id, 2022, 1500.55m)
        );

        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _container.DisposeAsync();
    }

    [Fact]
    public async Task GetAll_WeightAscending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("weight", "asc");

        Assert.Equal(
            new[] { 2, 3, 4, 1 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_WeightDescending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("weight", "desc");

        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_YearAscending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("year", "asc");

        Assert.Equal(
            new[] { 2, 3, 1, 4 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_YearDescending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("year", "desc");

        Assert.Equal(
            new[] { 1, 4, 2, 3 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_OwnerNameAscending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("ownerName", "asc");

        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_OwnerNameDescending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("ownerName", "desc");

        Assert.Equal(
            new[] { 4, 1, 2, 3 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_ManufacturerAscending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("manufacturer", "asc");

        // Honda, Mazda, Mazda, Toyota
        Assert.Equal(
            new[] { 3, 1, 2, 4 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_ManufacturerDescending_ReturnsCorrectOrder()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("manufacturer", "desc");

        // Toyota, Mazda, Mazda, Honda
        Assert.Equal(
            new[] { 4, 1, 2, 3 },
            result.Select(v => v.Id));
    }

    [Fact]
    public async Task GetAll_SameWeight_OrdersById()
    {
        var repository = new VehicleRepository(_context);

        var result = await repository.GetAllAsync("weight", "asc");

        var sameWeightVehicles = result
            .Where(v => v.Weight == 1500.55m)
            .Select(v => v.Id)
            .ToArray();

        Assert.Equal(
            new[] { 2, 3, 4 },
            sameWeightVehicles);
    }
}
