using System.Threading.Tasks;
using FleetManager.Models;
using FleetManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleService _vehicleService;

    public VehiclesController(VehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _vehicleService.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> Get(int id)
    {
        var vehicle = await _vehicleService.Get(id);

        if (vehicle == null)
            return NotFound();

        return Ok(vehicle);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Vehicle vehicle)
    {
        await _vehicleService.Add(vehicle);

        return CreatedAtAction(nameof(Get), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
            return BadRequest();

        var existingVehicle = await _vehicleService.Get(id);

        if (existingVehicle == null)
            return NotFound();

        await _vehicleService.Update(vehicle);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _vehicleService.Get(id);

        if (vehicle == null)
            return NotFound();

        await _vehicleService.Delete(id);

        return NoContent();
    }
}