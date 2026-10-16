namespace CreditWorks.Application.DTOs.Categories;

public sealed record CategoryResponse(
    int Id,
    string Name,
    string Icon,
    decimal MinWeight,
    decimal? MaxWeight);