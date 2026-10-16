using System.ComponentModel.DataAnnotations;

namespace CreditWorks.Application.DTOs.Vehicles;

public sealed class CreateVehicleRequest
{
    [Required]
    [StringLength(150, MinimumLength = 1)]
    public string OwnerName { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ManufacturerId { get; init; }

    [Range(1886, 9999)]
    public int YearOfManufacture { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Weight { get; init; }
}