using CreditWorks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.Api.Controllers;

[ApiController]
[Route("api/manufacturers")]
public sealed class ManufacturersController : ControllerBase
{
    private readonly IManufacturerRepository _manufacturers;

    public ManufacturersController(
        IManufacturerRepository manufacturers)
    {
        _manufacturers = manufacturers;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var manufacturers = await _manufacturers.GetAllAsync(
            cancellationToken);

        return Ok(manufacturers.Select(m => new
        {
            m.Id,
            m.Name
        }));
    }
}