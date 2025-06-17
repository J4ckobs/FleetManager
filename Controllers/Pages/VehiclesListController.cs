using Microsoft.AspNetCore.Mvc;
using FleetManager.Services;
using FleetManager.Models;

namespace FleetManager.Controllers
{
    [ApiController]
    [Route("page/vehicles", Name = "VehiclesList")]
    public class VehiclesListController : Controller
    {
        private readonly VehicleService _vehicleService;

        public VehiclesListController(VehicleService vehisleService)
        {
            _vehicleService = vehisleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var vehicles = await _vehicleService.GetAll();

                return View("~/Views/Pages/VehiclesList.cshtml", vehicles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Błąd podczas ładowania danych: {ex.Message}";
                return View();
            }
        }
    }
}