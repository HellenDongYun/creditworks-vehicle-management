using CreditWorks.Domain.Entities;

namespace CreditWorks.Application.Interfaces;

public interface IManufacturerRepository
{
    Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default);
}