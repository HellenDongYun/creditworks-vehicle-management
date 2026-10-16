
using CreditWorks.Application.DTOs.Vehicles;
using CreditWorks.Application.Exceptions;
using CreditWorks.Application.Interfaces;
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Services;

namespace CreditWorks.Application.Services;

public sealed class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicles;
    private readonly IManufacturerRepository _manufacturers;
    private readonly ICategoryRepository _categories;
    private readonly CategoryResolver _resolver = new();

    public VehicleService(
        IVehicleRepository vehicles,
        IManufacturerRepository manufacturers,
        ICategoryRepository categories)
    {
        _vehicles = vehicles;
        _manufacturers = manufacturers;
        _categories = categories;
    }

    public async Task<IReadOnlyList<VehicleResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var vehicles = await _vehicles.GetAllAsync(cancellationToken);
        var categories = await _categories.GetAllAsync(cancellationToken);

        return vehicles
            .Select(v => MapResponse(v, categories))
            .ToList();
    }

    public async Task<VehicleResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicles.GetByIdAsync(
            id, cancellationToken);

        if (vehicle is null)
        {
            throw new ResourceNotFoundException(
                $"Vehicle with ID {id} was not found.");
        }

        var categories = await _categories.GetAllAsync(cancellationToken);

        return MapResponse(vehicle, categories);
    }

    public async Task<VehicleResponse> CreateAsync(
        CreateVehicleRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate manufacturer existence
        var manufacturerExists = await _manufacturers.ExistsAsync(
            request.ManufacturerId, cancellationToken);

        if (!manufacturerExists)
        {
            throw new ResourceNotFoundException(
                $"Manufacturer with ID {request.ManufacturerId} was not found.");
        }

        // 2. Construct the domain entity
        var vehicle = new Vehicle(
            request.OwnerName,
            request.ManufacturerId,
            request.YearOfManufacture,
            request.Weight);

        // 3. Validate category assignment before saving
        var categories = await _categories.GetAllAsync(cancellationToken);
        _resolver.Resolve(vehicle.Weight, categories);

        // 4. Persist the vehicle
        await _vehicles.AddAsync(vehicle, cancellationToken);
        await _vehicles.SaveChangesAsync(cancellationToken);

        // 5. Reload to populate manufacturer navigation
        var savedVehicle = await _vehicles.GetByIdAsync(
            vehicle.Id, cancellationToken);

        if (savedVehicle is null)
        {
            throw new InvalidOperationException(
                "The saved vehicle could not be retrieved.");
        }

        return MapResponse(savedVehicle, categories);
    }

    private VehicleResponse MapResponse(
        Vehicle vehicle,
        IReadOnlyCollection<VehicleCategory> categories)
    {
        var category = _resolver.Resolve(vehicle.Weight, categories);

        return new VehicleResponse(
            vehicle.Id,
            vehicle.OwnerName,
            vehicle.ManufacturerId,
            vehicle.Manufacturer.Name,
            vehicle.YearOfManufacture,
            vehicle.Weight,
            category.Id,
            category.Name,
            category.Icon
        );
    }
}
