using FleetManager.Models;

namespace FleetManager.Services;

public class VehicleService
{
    public FileDataService<Vehicle> dataService;

    public int Index = 4;

    public VehicleService(FileDataService<Vehicle> _dataService)
    {
        dataService = _dataService;
    }

    public IEnumerable<Vehicle> GetAll() => dataService.GetAll();
    public Vehicle? Get(int id) => dataService.GetByPredictateAsync(x => x.Id == id);
    public async Task Add(Vehicle vehicle) => await dataService.AddAsync(vehicle);
    public async Task Delete(int id) => await dataService.RemoveByPredictateAsync(x => x.Id == id);
    public async Task Update(Vehicle vehicle) => await dataService.UpdateAsync(x => x.Id == vehicle.Id, vehicle);
}