using CreditWorks.Application.DTOs.Categories;

namespace CreditWorks.Application.Interfaces;

public interface ICategoryManagementService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryResponse>> UpdateConfigurationAsync(
        CategoryConfigurationRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryResponse>> CreateAsync(
        CategoryItemRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryResponse>> UpdateAsync(
        int id,
        CategoryItemRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryResponse>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}