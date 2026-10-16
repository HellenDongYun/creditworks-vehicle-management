using CreditWorks.Domain.Entities;

namespace CreditWorks.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<VehicleCategory>> GetAllAsync(
        CancellationToken cancellationToken = default);
}