using System.Threading.Tasks;
using FleetManager.Models;

namespace FleetManager.Services;

public class VehicleService
{
    public FileDataService<Vehicle> vehicleService;

    public VehicleService(FileDataService<Vehicle> _vehicleService)
    {
        vehicleService = _vehicleService;
    }

    public async Task<IEnumerable<Vehicle>> GetAll() => await vehicleService.GetAllAsync();
    public async Task<IEnumerable<Vehicle>> GetAvaliableVehicles() => await vehicleService.GetSelectedAsync(vehicle => vehicle.Status == VehicleStatus.Available);
    public async Task<Vehicle?> Get(int id) => await vehicleService.GetByPredictateAsync(vehicle => vehicle.Id == id);
    public async Task Add(Vehicle vehicle) => await vehicleService.AddAsync(vehicle);
    public async Task Delete(int id) => await vehicleService.RemoveByPredictateAsync(vehicle => vehicle.Id == id);
    public async Task Update(Vehicle vehicle) => await vehicleService.UpdateAsync(v => v.Id == vehicle.Id, vehicle);
}