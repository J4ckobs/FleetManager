using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;

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
                    TotalVehicles = vehicles.Count(),
                    TotalDrivers = drivers.Count(),
                    ActiveVehicles = vehicles.Count(v => v.Status == 0),
                    AvailableDrivers = drivers.Count(d => d.AssignedVehicleLicensePlate == null),
                    RecentVehicles = vehicles.OrderByDescending(v => v.Id).Take(5).ToList(),
                    RecentDrivers = drivers.OrderByDescending(d => d.Id).Take(5).ToList()
                };

                return View("~/Views/Index.cshtml", dashboardData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Błąd podczas ładowania danych: {ex.Message}";
                return View(new DashboardViewModel());
            }
        }
    }

    // ViewModel dla dashboardu
    public class DashboardViewModel
    {
        public int TotalVehicles { get; set; }
        public int TotalDrivers { get; set; }
        public int ActiveVehicles { get; set; }
        public int AvailableDrivers { get; set; }
        public List<Vehicle> RecentVehicles { get; set; } = new();
        public List<Driver> RecentDrivers { get; set; } = new();
    }
}