using System.Threading.Tasks;
using FleetManager.Models;
using FleetManager.Services;
using Microsoft.AspNetCore.Mvc;
using Route = FleetManager.Models.Route;

namespace FleetManager.Controllers;

[ApiController]
[Route("api/routes")]
public class RouteController : ControllerBase
{
    private readonly RouteService _routeService;

    public RouteController(RouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _routeService.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> Get(int id)
    {
        var driver = await _routeService.Get(id);

        if (driver == null)
            return NotFound();

        return Ok(driver);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Route route)
    {
        await _routeService.Add(route);

        return CreatedAtAction(nameof(Get), new { id = route.Id }, route);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Route route)
    {
        if (id != route.Id)
            return BadRequest();

        var existingRoute = await _routeService.Get(id);

        if (existingRoute == null)
            return NotFound();

        existingRoute.Departure = route.Departure;
        existingRoute.Destination = route.Destination;

        existingRoute.DepartureCoords = route.DepartureCoords;
        existingRoute.DestinationCoords = route.DestinationCoords;

        existingRoute.AssignedVehicle = route.AssignedVehicle;
        existingRoute.AssignedDriver = route.AssignedDriver;

        existingRoute.Distance = route.Distance;

        existingRoute.DepartureTime = route.DepartureTime;
        existingRoute.ArrivalTime = route.ArrivalTime;

        await _routeService.Update(existingRoute);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _routeService.Get(id);

        if (driver == null)
            return NotFound();

        await _routeService.Delete(id);

        return Ok();
    }
}