using CreditWorks.Application.DTOs.Vehicles;

namespace CreditWorks.Application.Interfaces;

public interface IVehicleService
{
    Task<IReadOnlyList<VehicleResponse>> GetAllAsync(
        string sortBy = "ownerName",
        string direction = "asc",
        CancellationToken cancellationToken = default);

    Task<VehicleResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<VehicleResponse> CreateAsync(
        CreateVehicleRequest request,
        CancellationToken cancellationToken = default);
}