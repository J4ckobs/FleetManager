using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;

namespace FleetManager.Controllers
{
    [ApiController]
    [Route("page/drivers", Name = "DriversList")]
    public class DriversListController : Controller
    {
        private readonly DriverService _driverService;

        public DriversListController(DriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var drivers = (await _driverService.GetAll()).ToList();

                return View("~/Views/Pages/DriversList.cshtml", drivers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Błąd podczas ładowania danych: {ex.Message}";
                return View();
            }
        }
    }
}