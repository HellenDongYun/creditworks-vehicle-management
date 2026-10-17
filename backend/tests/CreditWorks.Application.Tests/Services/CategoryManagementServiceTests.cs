
using CreditWorks.Application.DTOs.Categories;
using CreditWorks.Application.Interfaces;
using CreditWorks.Application.Services;
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.ValueObjects;
using Xunit;

namespace CreditWorks.Application.Tests.Services;

public sealed class CategoryManagementServiceTests
{
    [Fact]
    public async Task UpdateConfiguration_ValidRanges_SavesSuccessfully()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryManagementService(repository);

        var result = await service.UpdateConfigurationAsync(
            CreateRequest(
                (null, "Light", "car", 0m, 500m),
                (null, "Medium", "truck", 500m, 2500m),
                (null, "Heavy", "truck", 2500m, null)));

        Assert.True(repository.SaveCalled);
        Assert.Equal(3, result.Count);
        Assert.Equal(500m, result[0].MaxWeight);
    }

    [Fact]
    public async Task UpdateConfiguration_Gap_RejectsWithoutSaving()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryManagementService(repository);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateConfigurationAsync(
                CreateRequest(
                    (null, "Light", "car", 0m, 400m),
                    (null, "Medium", "truck", 500m, null))));

        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public async Task UpdateConfiguration_Overlap_RejectsWithoutSaving()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryManagementService(repository);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateConfigurationAsync(
                CreateRequest(
                    (null, "Light", "car", 0m, 700m),
                    (null, "Medium", "truck", 500m, null))));

        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public async Task UpdateConfiguration_DuplicateNames_RejectsWithoutSaving()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryManagementService(repository);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateConfigurationAsync(
                CreateRequest(
                    (null, "Light", "car", 0m, 500m),
                    (null, " light ", "truck", 500m, null))));

        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public async Task UpdateConfiguration_FirstRangeNotZero_Rejects()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryManagementService(repository);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateConfigurationAsync(
                CreateRequest(
                    (null, "Light", "car", 1m, null))));

        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public async Task UpdateConfiguration_LastRangeHasMaximum_Rejects()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryManagementService(repository);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateConfigurationAsync(
                CreateRequest(
                    (null, "Light", "car", 0m, 500m))));

        Assert.False(repository.SaveCalled);
    }

    private static CategoryConfigurationRequest CreateRequest(
        params (int? Id, string Name, string Icon,
            decimal Min, decimal? Max)[] items)
    {
        return new CategoryConfigurationRequest
        {
            Categories = items.Select(item => new CategoryItemRequest
            {
                Id = item.Id,
                Name = item.Name,
                Icon = item.Icon,
                MinWeight = item.Min,
                MaxWeight = item.Max
            }).ToList()
        };
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        private List<VehicleCategory> _categories = new();

        public bool SaveCalled { get; private set; }

        public Task<IReadOnlyList<VehicleCategory>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<VehicleCategory>>(
                _categories);
        }

        public Task ReplaceConfigurationAsync(
            IReadOnlyCollection<CategoryDefinition> definitions,
            CancellationToken cancellationToken = default)
        {
            SaveCalled = true;

            _categories = definitions.Select(d =>
                new VehicleCategory(
                    d.Name,
                    d.Icon,
                    new WeightRange(d.MinWeight, d.MaxWeight)))
                .ToList();

            return Task.CompletedTask;
        }
    }
}
