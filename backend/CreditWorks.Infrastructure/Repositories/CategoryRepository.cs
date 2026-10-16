
using System.Data;
using CreditWorks.Application.DTOs.Categories;
using CreditWorks.Application.Interfaces;
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;
using CreditWorks.Domain.Services;
using CreditWorks.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly CreditWorksDbContext _context;
    private readonly CategoryRangeValidator _validator = new();

    public CategoryRepository(CreditWorksDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VehicleCategory>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleCategories
            .AsNoTracking()
            .OrderBy(c => c.Range.MinWeight)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceConfigurationAsync(
        IReadOnlyCollection<CategoryDefinition> definitions,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        var existing = await _context.VehicleCategories
            .ToListAsync(cancellationToken);

        var existingById = existing.ToDictionary(c => c.Id);

        var incomingIds = definitions
            .Where(d => d.Id.HasValue)
            .Select(d => d.Id!.Value)
            .ToList();

        if (incomingIds.Count != incomingIds.Distinct().Count())
        {
            throw new DomainValidationException(
                "Duplicate category IDs are not allowed.");
        }

        foreach (var id in incomingIds)
        {
            if (!existingById.ContainsKey(id))
            {
                throw new DomainValidationException(
                    $"Category {id} does not exist.");
            }
        }

        var retainedIds = incomingIds.ToHashSet();

        var removed = existing
            .Where(c => !retainedIds.Contains(c.Id))
            .ToList();

        _context.VehicleCategories.RemoveRange(removed);

        foreach (var definition in definitions)
        {
            var range = new WeightRange(
                definition.MinWeight,
                definition.MaxWeight);

            if (definition.Id.HasValue)
            {
                var category = existingById[definition.Id.Value];

                category.Update(
                    definition.Name,
                    definition.Icon,
                    range);
            }
            else
            {
                var category = new VehicleCategory(
                    definition.Name,
                    definition.Icon,
                    range);

                _context.VehicleCategories.Add(category);
            }
        }

        var finalCategories = _context.ChangeTracker
            .Entries<VehicleCategory>()
            .Where(e => e.State != EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        _validator.Validate(finalCategories);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
