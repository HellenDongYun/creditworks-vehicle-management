
using CreditWorks.Application.DTOs.Categories;
using CreditWorks.Domain.Entities;
using CreditWorks.Infrastructure.Persistence;
using CreditWorks.Infrastructure.Persistence.Repositories;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace CreditWorks.IntegrationTests.Repositories;

public sealed class CategoryRepositoryTests
    : IClassFixture<CategoryDatabaseFixture>
{
    private readonly CategoryDatabaseFixture _fixture;

    public CategoryRepositoryTests(CategoryDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ReplaceConfiguration_CreatesCategories()
    {
        await using var context = CreateContext();
        await ResetDatabaseAsync(context);

        var repository = new CategoryRepository(context);

        await repository.ReplaceConfigurationAsync(
            new[]
            {
                Define(null, "Light", "car", 0m, 500m),
                Define(null, "Medium", "truck", 500m, 2500m),
                Define(null, "Heavy", "truck", 2500m, null)
            });

        var categories = await repository.GetAllAsync();

        Assert.Equal(3, categories.Count);
        Assert.Equal("Light", categories[0].Name);
        Assert.Equal("Medium", categories[1].Name);
        Assert.Equal("Heavy", categories[2].Name);
    }

    [Fact]
    public async Task ReplaceConfiguration_UpdatesWithoutChangingIds()
    {
        await using var context = CreateContext();
        await ResetDatabaseAsync(context);

        var repository = new CategoryRepository(context);

        await repository.ReplaceConfigurationAsync(
            new[]
            {
                Define(null, "Light", "car", 0m, 500m),
                Define(null, "Heavy", "truck", 500m, null)
            });

        var before = await repository.GetAllAsync();

        await repository.ReplaceConfigurationAsync(
            new[]
            {
                Define(before[0].Id, "Light Vehicle",
                    "car-front", 0m, 600m),
                Define(before[1].Id, "Heavy",
                    "truck", 600m, null)
            });

        var after = await repository.GetAllAsync();

        Assert.Equal(before[0].Id, after[0].Id);
        Assert.Equal(before[1].Id, after[1].Id);
        Assert.Equal("Light Vehicle", after[0].Name);
        Assert.Equal("car-front", after[0].Icon);
        Assert.Equal(600m, after[0].Range.MaxWeight);
    }

    [Fact]
    public async Task ReplaceConfiguration_InvalidId_DoesNotChangeDatabase()
    {
        await using var context = CreateContext();
        await ResetDatabaseAsync(context);

        var repository = new CategoryRepository(context);

        await repository.ReplaceConfigurationAsync(
            new[]
            {
                Define(null, "Light", "car", 0m, 500m),
                Define(null, "Heavy", "truck", 500m, null)
            });

        var before = await repository.GetAllAsync();

        await Assert.ThrowsAsync<
            CreditWorks.Domain.Exceptions.DomainValidationException>(
            () => repository.ReplaceConfigurationAsync(
                new[]
                {
                    Define(999999, "Invalid", "car", 0m, null)
                }));

        await using var verificationContext = CreateContext();
        var verificationRepository =
            new CategoryRepository(verificationContext);

        var after = await verificationRepository.GetAllAsync();

        Assert.Equal(before.Count, after.Count);
        Assert.Equal(
            before.Select(c => c.Name),
            after.Select(c => c.Name));
    }

    [Fact]
    public async Task ReplaceConfiguration_DuplicateIds_DoesNotChangeDatabase()
    {
        await using var context = CreateContext();
        await ResetDatabaseAsync(context);

        var repository = new CategoryRepository(context);

        await repository.ReplaceConfigurationAsync(
            new[]
            {
                Define(null, "Light", "car", 0m, 500m),
                Define(null, "Heavy", "truck", 500m, null)
            });

        var before = await repository.GetAllAsync();
        var duplicateId = before[0].Id;

        await Assert.ThrowsAsync<
            CreditWorks.Domain.Exceptions.DomainValidationException>(
            () => repository.ReplaceConfigurationAsync(
                new[]
                {
                    Define(duplicateId, "First", "car", 0m, 500m),
                    Define(duplicateId, "Second", "truck", 500m, null)
                }));

        await using var verificationContext = CreateContext();
        var after = await new CategoryRepository(verificationContext)
            .GetAllAsync();

        Assert.Equal(
            before.Select(c => c.Name),
            after.Select(c => c.Name));
    }

    private CreditWorksDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CreditWorksDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        return new CreditWorksDbContext(options);
    }

    private static async Task ResetDatabaseAsync(
        CreditWorksDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    private static CategoryDefinition Define(
        int? id,
        string name,
        string icon,
        decimal min,
        decimal? max)
    {
        return new CategoryDefinition(id, name, icon, min, max);
    }
}


public sealed class CategoryDatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container =
        new MsSqlBuilder(
                "mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

    private readonly string _databaseName =
        $"CreditWorks_CategoryTests_{Guid.NewGuid():N}";

    public string ConnectionString
    {
        get
        {
            var builder =
                new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(
                    _container.GetConnectionString());

            builder.InitialCatalog = _databaseName;

            return builder.ConnectionString;
        }
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

