using CreditWorks.Application.Interfaces;
using CreditWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
}