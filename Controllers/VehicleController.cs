using FleetManager.Models;
using FleetManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FleetManager.Controllers;

[ApiController]
[Route("[controller]")]
public class VehiclesController : ControllerBase
{

    [HttpGet]
    public IEnumerable<Vehicle> GetAll()
    {
        return VehicleService.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<Vehicle> Get(int id)
    {
        var vehicle = VehicleService.Get(id);

        if (vehicle == null)
            return NotFound();

        return vehicle;
    }

    [HttpPost]
    public ActionResult Create(Vehicle vehicle)
    {
        VehicleService.Add(vehicle);

        return CreatedAtAction(nameof(Get), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
            return BadRequest();

        var existingVehicle = VehicleService.Get(id);

        if (existingVehicle == null)
            return NotFound();

        VehicleService.Update(vehicle);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var vehicle = VehicleService.Get(id);

        if (vehicle == null)
            return NotFound();

        VehicleService.Delete(id);

        return NoContent();
    }
}