using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;
using FleetManager.ViewModels;

namespace FleetManager.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly VehicleService _vehicleService;
        private readonly DriverService _driverService;

        public HomeController(VehicleService vehicleService, DriverService driverService)
        {
            _vehicleService = vehicleService;
            _driverService = driverService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Pobierz podstawowe statystyki
                var vehicles = await _vehicleService.GetAll();
                var drivers = await _driverService.GetAll();

                var dashboardData = new DashboardViewModel
                {
                    TotalDrivers = drivers.Count(),
                    AvailableDrivers = drivers.Count(d => d.RouteId == -1),
                    RecentDrivers = drivers.OrderByDescending(d => d.Id).Take(5).ToList(),
                    
                    TotalVehicles = vehicles.Count(),
                    ActiveVehicles = vehicles.Count(v => v.Status == 0),
                    RecentVehicles = vehicles.OrderByDescending(v => v.Id).Take(5).ToList()
                };

                return View("~/Views/Index.cshtml", dashboardData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error occured while trying to load the data: {ex.Message}";
                return View(new DashboardViewModel());
            }
        }
    }
}