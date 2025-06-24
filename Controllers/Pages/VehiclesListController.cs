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
        public async Task<IActionResult> Index(string? action = null)
        {
            try
            {
                var vehicles = await _vehicleService.GetAll();

                ViewBag.ShowAddForm = action == "add";

                return View("~/Views/Pages/VehiclesList.cshtml", vehicles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error occured while trying to load the data: {ex.Message}";
                return View();
            }
        }
    }
}