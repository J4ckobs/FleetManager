using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;
using FleetManager.ViewModels;

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
                ViewBag.Error = $"Error occured while trying to load the data: {ex.Message}";
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
            }).ToList();

            var vehiclesList = vehicles.Select(v => new VehicleViewModel
            {
                Id = v.Id,
                Brand = v.Brand,
                Model = v.Model,
                LicensePlate = v.LicensePlate
            }).ToList();

            return Ok(new DriversVehiclesRequest
            {
                AvaliableDrivers = driversList,
                AvaliableVehicles = vehiclesList
            });
        }

        [HttpGet("current-driver-and-vehicle/{driverId}/{vehicleId}")]
        public async Task<ActionResult<CurrentDriverAndVehicle>> GetCurrentDriverAndVehicle(int driverId, int vehicleId)
        {
            var driver = await _driverService.Get(driverId);
            var vehicle = await _vehicleService.Get(vehicleId);

            var currentDriver = new DriverViewModel();
            var currentVehicle = new VehicleViewModel();

            if (driver != null)
                currentDriver = new DriverViewModel
                {
                    Id = driver.Id,
                    Name = driver.FirstName,
                    LastName = driver.LastName,
                };

            if (vehicle != null)
                currentVehicle = new VehicleViewModel
                {
                    Id = vehicle.Id,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    LicensePlate = vehicle.LicensePlate
                };

            return Ok(new CurrentDriverAndVehicle
            {
                CurrentDriver = currentDriver,
                CurrentVehicle = currentVehicle
            });
        }
        [HttpPost("validate-driver-and-vehicle")]
        public async Task<ActionResult> CreateRouteValidateDriverAndVehicle([FromBody] Models.Route route)
        {
            var succesfullAdd = await _routeService.Add(route);

            if (!succesfullAdd)
                return BadRequest("Unbale to update route");

            var driverValidateStatus = await ValidateDriver(route.AssignedDriver, route.Id);
            var vehicleValidateStatus = await ValidateVehicle(route.AssignedVehicle, route.Id);

            if (!driverValidateStatus)
                return BadRequest("Unable to assign driver to route");

            if (!vehicleValidateStatus)
                return BadRequest("Unable to assign vehicle to route");

            return Ok();
        }

        [HttpPut("validate-driver-and-vehicle/{id}")]
        public async Task<ActionResult> UpdateRouteValidateDriverAndVehicle([FromBody] Models.Route route)
        {
            var succesfullUpdate = await _routeService.Update(route);

            if (!succesfullUpdate)
                return BadRequest("Unbale to update route");

            var driverValidateStatus = await ValidateDriver(route.AssignedDriver, route.Id);
            var vehicleValidateStatus = await ValidateVehicle(route.AssignedVehicle, route.Id);

            if (!driverValidateStatus)
                return BadRequest("Unable to assign driver to route");

            if (!vehicleValidateStatus)
                return BadRequest("Unable to assign vehicle to route");

            return Ok();
        }

        private async Task<bool> ValidateDriver(int assignedDriver, int routeId)
        {
            if (assignedDriver == -1)
                return false;

            var drivers = await _driverService.GetAll();

            var currentDriver = drivers.FirstOrDefault(x => x.RouteId == routeId);

            if (currentDriver != null && currentDriver.Id == assignedDriver)
                return true;

            if (currentDriver != null)
            {
                currentDriver.RouteId = -1;
                await _driverService.Update(currentDriver);
            }

            try
            {
                var newDriver = drivers.FirstOrDefault(x => x.Id == assignedDriver);

                if (newDriver == null)
                    return false;

                newDriver.RouteId = routeId;

                await _driverService.Update(newDriver);
                return true;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<bool> ValidateVehicle(int assignedVehicle, int routeId)
        {
            if (assignedVehicle == -1)
                return false;

            var vehicles = await _vehicleService.GetAll();

            var currentVehicle = vehicles.FirstOrDefault(x => x.RouteId == routeId);

            if (currentVehicle != null && currentVehicle.Id == assignedVehicle)
                return true;

            if (currentVehicle != null)
            {
                currentVehicle.RouteId = -1;
                currentVehicle.Status = VehicleStatus.Available;

                await _vehicleService.Update(currentVehicle);
            }

            try
            {
                var newVehicle = vehicles.FirstOrDefault(x => x.Id == assignedVehicle);

                if (newVehicle == null)
                    return false;

                newVehicle.RouteId = routeId;
                newVehicle.Status = VehicleStatus.Assigned;

                await _vehicleService.Update(newVehicle);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}