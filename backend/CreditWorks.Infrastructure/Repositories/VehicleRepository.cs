
using CreditWorks.Application.Interfaces;
using CreditWorks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.Infrastructure.Persistence.Repositories;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly CreditWorksDbContext _context;

    public VehicleRepository(CreditWorksDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(
        string sortBy = "ownerName",
        string direction = "asc",
        CancellationToken cancellationToken = default)
    {
        IQueryable<Vehicle> query = _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Manufacturer);

        var descending = direction.Equals(
            "desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLowerInvariant() switch
        {
            "ownername" => descending
                ? query.OrderByDescending(v => v.OwnerName)
                    .ThenBy(v => v.Id)
                : query.OrderBy(v => v.OwnerName)
                    .ThenBy(v => v.Id),

            "manufacturer" => descending
                ? query.OrderByDescending(v => v.Manufacturer.Name)
                    .ThenBy(v => v.Id)
                : query.OrderBy(v => v.Manufacturer.Name)
                    .ThenBy(v => v.Id),

            "year" => descending
                ? query.OrderByDescending(v => v.YearOfManufacture)
                    .ThenBy(v => v.Id)
                : query.OrderBy(v => v.YearOfManufacture)
                    .ThenBy(v => v.Id),

            "weight" => descending
                ? query.OrderByDescending(v => v.Weight)
                    .ThenBy(v => v.Id)
                : query.OrderBy(v => v.Weight)
                    .ThenBy(v => v.Id),

            _ => query.OrderBy(v => v.OwnerName)
                .ThenBy(v => v.Id)
        };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Vehicle?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Manufacturer)
            .FirstOrDefaultAsync(
                v => v.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(
            vehicle, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
