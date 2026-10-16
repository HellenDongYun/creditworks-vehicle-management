using System.ComponentModel.DataAnnotations;

namespace CreditWorks.Application.DTOs.Categories;

public sealed class CategoryConfigurationRequest
{
    [Required]
    [MinLength(1)]
    public List<CategoryItemRequest> Categories { get; init; } = new();
}