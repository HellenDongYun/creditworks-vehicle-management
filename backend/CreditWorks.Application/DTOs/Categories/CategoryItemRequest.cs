using System.ComponentModel.DataAnnotations;

namespace CreditWorks.Application.DTOs.Categories;

public sealed class CategoryItemRequest
{
    public int? Id { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Icon { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal MinWeight { get; init; }

    public decimal? MaxWeight { get; init; }
}