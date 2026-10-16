using CreditWorks.Application.DTOs.Vehicles;
using CreditWorks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VehicleResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetAllAsync(
            cancellationToken);

        return Ok(vehicles);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.GetByIdAsync(
            id, cancellationToken);

        return Ok(vehicle);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create(
        [FromBody] CreateVehicleRequest request,
        CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.CreateAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = vehicle.Id },
            vehicle);
    }
}