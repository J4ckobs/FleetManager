using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;

namespace FleetManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly VehicleService _vehicleService;
        private readonly DriverService _driverService;

        public HomeController(VehicleService vehicleService, DriverService driverService)
        {
            _vehicleService = vehicleService;
            _driverService = driverService;
        }

        public IActionResult Index()
        {
            try
            {
                // Pobierz podstawowe statystyki
                var vehicles = _vehicleService.GetAll();
                var drivers = _driverService.GetAll();

                var dashboardData = new DashboardViewModel
                {
                    TotalVehicles = vehicles.Count(),
                    TotalDrivers = drivers.Count(),
                    ActiveVehicles = vehicles.Count(v => v.Status == "Active"),
                    //AvailableDrivers = drivers.Count(d => d.Status == "Available"),
                    RecentVehicles = vehicles.OrderByDescending(v => v.Id).Take(5).ToList(),
                    RecentDrivers = drivers.OrderByDescending(d => d.Id).Take(5).ToList()
                };

                return View(dashboardData);
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