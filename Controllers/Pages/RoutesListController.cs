using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;
using System.Diagnostics.Eventing.Reader;

namespace FleetManager.Controllers
{
    [ApiController]
    [Route("page/routes", Name = "RoutesList")]
    public class RoutesListController : Controller
    {
        private readonly DriverService _driverService;
        private readonly VehicleService _vehicleService;
        private readonly RouteService _routeService;

        public RoutesListController(DriverService driverService, VehicleService vehicleService, RouteService routeService)
        {
            _driverService = driverService;
            _vehicleService = vehicleService;
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var routes = await _routeService.GetAll();

                foreach (var route in routes)
                {
                    var departureParts = route.Departure?.Split(',').Select(p => p.Trim()).ToArray();

                    if (departureParts?.Length >= 4)
                        route.Departure = string.Join(", ", new[] { 0, 1, departureParts.Length - 2, departureParts.Length - 1 }.Select(i => departureParts[i]));

                    var destinationParts = route.Destination?.Split(',').Select(p => p.Trim()).ToArray();

                    if (destinationParts?.Length >= 4)
                        route.Destination = string.Join(", ", new[] { 0, 1, destinationParts.Length - 2, destinationParts.Length - 1 }.Select(i => destinationParts[i]));
                }

                return View("~/Views/Pages/RoutesList.cshtml", routes);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Błąd podczas ładowania danych: {ex.Message}";
                return View();
            }
        }

        [HttpGet("avaliable-drivers-and-vehicles")]
        public async Task<ActionResult<DriversVehiclesRequest>> GetAvaliableDriversAndVehicles()
        {
            var drivers = await _driverService.GetAvaliableDrivers();
            var vehicles = await _vehicleService.GetAvaliableVehicles();

            var driversList = drivers.Select(d => new DriverViewModel
            {
                Id = d.Id,
                Name = d.FirstName,
                LastName = d.LastName,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email

            }).ToList();

            var vehiclesList = vehicles.Select(v => new VehicleViewModel
            {
                Id = v.Id,
                Brand = v.Brand,
                Model = v.Model,
                Mileage = v.Mileage,
                LicensePlate = v.LicensePlate
            }).ToList();

            return Ok(new DriversVehiclesRequest
            {
                AvaliableDrivers = driversList,
                AvaliableVehicles = vehiclesList
            });
        }
    }

    public class DriverViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class VehicleViewModel
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Mileage { get; set; }
        public string? LicensePlate { get; set; }
    }

    public class DriversVehiclesRequest
    {
        public List<DriverViewModel>? AvaliableDrivers { get; set; }
        public List<VehicleViewModel>? AvaliableVehicles { get; set; }
    }
}