using CreditWorks.Domain.Entities;

namespace CreditWorks.Application.Interfaces;

public interface IVehicleRepository
{
    Task<IReadOnlyList<Vehicle>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Vehicle?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}