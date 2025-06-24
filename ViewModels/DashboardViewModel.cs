using FleetManager.Models;

namespace FleetManager.ViewModels
{
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