using CreditWorks.Application.DTOs.Categories;
using CreditWorks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryManagementService _service;

    public CategoriesController(
        ICategoryManagementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>>
        UpdateConfiguration(
            [FromBody] CategoryConfigurationRequest request,
            CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateConfigurationAsync(
            request, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> Create(
        [FromBody] CategoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var categories = await _service.CreateAsync(
            request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, categories);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> Update(
        int id,
        [FromBody] CategoryItemRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(
            id, request, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(
            id, cancellationToken));
    }
}