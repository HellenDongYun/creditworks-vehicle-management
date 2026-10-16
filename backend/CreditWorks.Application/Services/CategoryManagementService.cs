
using CreditWorks.Application.DTOs.Categories;
using CreditWorks.Application.Interfaces;
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;
using CreditWorks.Application.Exceptions;

namespace CreditWorks.Application.Services;

public sealed class CategoryManagementService
    : ICategoryManagementService
{
    private readonly ICategoryRepository _repository;
    private readonly CategoryRangeValidator _validator = new();

    public CategoryManagementService(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(
            cancellationToken);

        return categories.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<CategoryResponse>>
        UpdateConfigurationAsync(
            CategoryConfigurationRequest request,
            CancellationToken cancellationToken = default)
    {
        if (request?.Categories is null ||
            request.Categories.Count == 0)
        {
            throw new DomainValidationException(
                "At least one category is required.");
        }

        var definitions = request.Categories.Select(c =>
            new CategoryDefinition(
                c.Id,
                c.Name,
                c.Icon,
                c.MinWeight,
                c.MaxWeight)).ToList();

        var duplicateNames = definitions
            .GroupBy(d => d.Name?.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .Any(g => g.Count() > 1);

        if (duplicateNames)
        {
            throw new DomainValidationException(
                "Category names must be unique.");
        }

        var candidateCategories = definitions.Select(d =>
            new VehicleCategory(
                d.Name,
                d.Icon,
                new WeightRange(
                    d.MinWeight,
                    d.MaxWeight))).ToList();

        _validator.Validate(candidateCategories);

        await _repository.ReplaceConfigurationAsync(
            definitions,
            cancellationToken);

        return await GetAllAsync(cancellationToken);
    }

    private static CategoryResponse Map(VehicleCategory category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Icon,
            category.Range.MinWeight,
            category.Range.MaxWeight);
    }
    
public async Task<IReadOnlyList<CategoryResponse>> CreateAsync(
    CategoryItemRequest request,
    CancellationToken cancellationToken = default)
{
    if (request is null)
        throw new DomainValidationException(
            "Category request is required.");

    if (request.Id.HasValue)
        throw new DomainValidationException(
            "New category must not specify an ID.");

    var existing = await GetAllAsync(cancellationToken);

    var items = existing.Select(ToRequest).ToList();
    items.Add(request);

    return await UpdateConfigurationAsync(
        new CategoryConfigurationRequest
        {
            Categories = items
        },
        cancellationToken);
}

public async Task<IReadOnlyList<CategoryResponse>> UpdateAsync(
    int id,
    CategoryItemRequest request,
    CancellationToken cancellationToken = default)
{
    if (request is null)
        throw new DomainValidationException(
            "Category request is required.");

    if (id <= 0)
        throw new DomainValidationException(
            "Category ID must be positive.");

    if (request.Id.HasValue && request.Id.Value != id)
        throw new DomainValidationException(
            "Request category ID does not match route ID.");

    var existing = await GetAllAsync(cancellationToken);

    if (!existing.Any(c => c.Id == id))
        throw new ResourceNotFoundException(
            $"Category {id} was not found.");

    var items = existing
        .Select(c => c.Id == id
            ? new CategoryItemRequest
            {
                Id = id,
                Name = request.Name,
                Icon = request.Icon,
                MinWeight = request.MinWeight,
                MaxWeight = request.MaxWeight
            }
            : ToRequest(c))
        .ToList();

    return await UpdateConfigurationAsync(
        new CategoryConfigurationRequest
        {
            Categories = items
        },
        cancellationToken);
}

public async Task<IReadOnlyList<CategoryResponse>> DeleteAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    if (id <= 0)
        throw new DomainValidationException(
            "Category ID must be positive.");

    var existing = await GetAllAsync(cancellationToken);

    if (!existing.Any(c => c.Id == id))
        throw new ResourceNotFoundException(
            $"Category {id} was not found.");

    var items = existing
        .Where(c => c.Id != id)
        .Select(ToRequest)
        .ToList();

    return await UpdateConfigurationAsync(
        new CategoryConfigurationRequest
        {
            Categories = items
        },
        cancellationToken);
}

private static CategoryItemRequest ToRequest(
    CategoryResponse category)
{
    return new CategoryItemRequest
    {
        Id = category.Id,
        Name = category.Name,
        Icon = category.Icon,
        MinWeight = category.MinWeight,
        MaxWeight = category.MaxWeight
    };
}

}
