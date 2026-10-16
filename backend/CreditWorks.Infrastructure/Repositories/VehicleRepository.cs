using CreditWorks.Application.Interfaces;
using CreditWorks.Domain.Entities;
using CreditWorks.Infrastructure.Persistence;
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
        CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Manufacturer)
            .OrderBy(v => v.Id)
            .ToListAsync(cancellationToken);
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