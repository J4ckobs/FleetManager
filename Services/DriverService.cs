using FleetManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Services;

public class DriverService
{
    public FleetContext context;
    public DriverService(FleetContext _context)
    {
        context = _context;
    }

    public async Task<IEnumerable<Driver>> GetAll() => await context.Drivers.ToListAsync();
    public async Task<IEnumerable<Driver>> GetAvaliableDrivers() => await context.Drivers.Where(driver => driver.RouteId == -1).ToListAsync();
    public async Task<Driver?> Get(int id) => await context.Drivers.FirstOrDefaultAsync(driver => driver.Id == id);
    public async Task<bool> Add(Driver driver)
    {
        var addedDriver = context.Drivers.Add(driver);

        if (addedDriver == null)
            return false;

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Delete(int id)
    {
        var driver = await context.Drivers.FindAsync(id);

        if (driver == null)
            return false;

        context.Drivers.Remove(driver);

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Update(Driver driver)
    {
        context.Drivers.Update(driver);

        return await context.SaveChangesAsync() > 0;
    }
}