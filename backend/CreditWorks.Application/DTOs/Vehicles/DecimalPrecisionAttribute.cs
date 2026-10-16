using System.ComponentModel.DataAnnotations;

namespace CreditWorks.Application.DTOs.Vehicles;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DecimalPrecisionAttribute : ValidationAttribute
{
    private readonly int _decimalPlaces;

    public DecimalPrecisionAttribute(int decimalPlaces)
    {
        _decimalPlaces = decimalPlaces;
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is not decimal number)
        {
            return new ValidationResult(
                "A valid decimal value is required.");
        }

        var rounded = Math.Round(
            number,
            _decimalPlaces,
            MidpointRounding.ToEven);

        if (number != rounded)
        {
            return new ValidationResult(
                $"Maximum {_decimalPlaces} decimal places allowed.");
        }

        return ValidationResult.Success;
    }
}