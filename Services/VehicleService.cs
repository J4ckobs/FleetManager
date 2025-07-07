using System.Threading.Tasks;
using FleetManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Services;

public class VehicleService
{
    public FleetContext context;

    public VehicleService(FleetContext _context)
    {
        context = _context;
    }

    public async Task<IEnumerable<Vehicle>> GetAll() =>
        await context.Vehicles.ToListAsync();
    public async Task<IEnumerable<Vehicle>> GetAvaliableVehicles() =>
        await context.Vehicles.Where(vehicle => vehicle.Status == VehicleStatus.Available).ToListAsync();
    public async Task<Vehicle?> Get(int id) =>
        await context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Id == id);
    public async Task<bool> Add(Vehicle vehicle)
    {
        var addedVehicle = context.Vehicles.Add(vehicle);

        if (addedVehicle == null)
            return false;

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Delete(int id)
    {
        var vehicle = await context.Vehicles.FindAsync(id);

        if (vehicle == null)
            return false;

        context.Vehicles.Remove(vehicle);

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Update(Vehicle vehicle)
    {
        context.Vehicles.Update(vehicle);

        return await context.SaveChangesAsync() > 0;
    }
}