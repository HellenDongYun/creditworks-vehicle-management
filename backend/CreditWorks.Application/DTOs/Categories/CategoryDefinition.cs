namespace CreditWorks.Application.DTOs.Categories;

public sealed record CategoryDefinition(
    int? Id,
    string Name,
    string Icon,
    decimal MinWeight,
    decimal? MaxWeight);