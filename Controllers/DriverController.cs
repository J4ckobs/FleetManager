using System.Threading.Tasks;
using FleetManager.Models;
using FleetManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriverController : ControllerBase
{
    private readonly DriverService _driverService;

    public DriverController(DriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _driverService.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> Get(int id)
    {
        var driver = await _driverService.Get(id);

        if (driver == null)
            return NotFound();

        return Ok(driver);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Driver driver)
    {
        await _driverService.Add(driver);

        return CreatedAtAction(nameof(Get), new { id = driver.Id }, driver);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Driver driver)
    {
        if (id != driver.Id)
            return BadRequest();

        var existingDriver = await _driverService.Get(id);

        if (existingDriver == null)
            return NotFound();

        existingDriver.FirstName = driver.FirstName;
        existingDriver.LastName = driver.LastName;
        existingDriver.LicenseNumber = driver.LicenseNumber;
        existingDriver.LicenseExpiryDate = driver.LicenseExpiryDate;
        existingDriver.PhoneNumber = driver.PhoneNumber;
        existingDriver.Email = driver.Email;

        await _driverService.Update(existingDriver);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _driverService.Get(id);

        if (driver == null)
            return NotFound();

        await _driverService.Delete(id);

        return NoContent();
    }
}