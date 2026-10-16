using System.ComponentModel.DataAnnotations;

namespace CreditWorks.Application.DTOs.Vehicles;

public sealed class NotFutureYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is not int year)
        {
            return new ValidationResult(
                "Year of manufacture is required.");
        }

        if (year > DateTime.UtcNow.Year)
        {
            return new ValidationResult(
                "Year of manufacture cannot be in the future.");
        }

        return ValidationResult.Success;
    }
}