using FleetManager.Models;

namespace FleetManager.Services;

public class DriverService
{
    public FileDataService<Driver> driverService;
    public DriverService(FileDataService<Driver> _driverService)
    {
        driverService = _driverService;
    }

    public async Task<IEnumerable<Driver>> GetAll() => await driverService.GetAllAsync();
    public async Task<Driver?> Get(int id) => await driverService.GetByPredictateAsync(x => x.Id == id);
    public async Task Add(Driver driver) => await driverService.AddAsync(driver);
    public async Task Delete(int id) => await driverService.RemoveByPredictateAsync(x => x.Id == id);
    public async Task Update(Driver driver) => await driverService.UpdateAsync(x => x.Id == driver.Id, driver);
}   