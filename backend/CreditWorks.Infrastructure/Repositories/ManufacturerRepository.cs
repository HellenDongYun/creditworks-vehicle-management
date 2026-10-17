using CreditWorks.Application.Interfaces;
using CreditWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CreditWorks.Domain.Entities;
namespace CreditWorks.Infrastructure.Persistence.Repositories;

public sealed class ManufacturerRepository
    : IManufacturerRepository
{
    private readonly CreditWorksDbContext _context;

    public ManufacturerRepository(CreditWorksDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Manufacturers
            .AnyAsync(m => m.Id == id, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Manufacturer>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Manufacturers
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

}