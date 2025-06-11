using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;
using System.Threading.Tasks;

namespace FleetManager.Controllers
{
    [Route("{controller}")]
    public class DriverBaseController : Controller
    {
        private readonly DriverService _driverService;

        public DriverBaseController(DriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDriverList()
        {
            try
            {
                var drivers = (await _driverService.GetAll()).OrderByDescending(x => x.Id).ToList();

                /*var dashboardData = new DashboardViewModel
                {
                    //TotalVehicles = vehicles.Count(),
                    TotalDrivers = drivers.Count(),
                    //ActiveVehicles = vehicles.Count(v => v.Status == 0),
                    AvailableDrivers = drivers.Count(d => d.AssignedVehicleLicensePlate == null),
                    //RecentVehicles = vehicles.OrderByDescending(v => v.Id).Take(5).ToList(),
                    RecentDrivers = drivers.OrderByDescending(d => d.Id).Take(5).ToList()
                };*/

                return View("~/Views/DriversPage.cshtml", drivers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Błąd podczas ładowania danych: {ex.Message}";
                return View();
            }
        }

        public IActionResult EditDriverProp()
        {
            return View("~/Views/EditDriver.cshtml");
        }
    }
}