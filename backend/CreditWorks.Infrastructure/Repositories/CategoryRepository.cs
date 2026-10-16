using CreditWorks.Application.Interfaces;
using CreditWorks.Domain.Entities;
using CreditWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly CreditWorksDbContext _context;

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
}