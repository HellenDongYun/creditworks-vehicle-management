namespace CreditWorks.Application.DTOs.Vehicles;

public sealed record VehicleResponse(
    int Id,
    string OwnerName,
    int ManufacturerId,
    string ManufacturerName,
    int YearOfManufacture,
    decimal Weight,
    int CategoryId,
    string CategoryName,
    string CategoryIcon
);