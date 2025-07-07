using FleetManager.Models;
using Microsoft.EntityFrameworkCore;
using Route = FleetManager.Models.Route;

namespace FleetManager.Services;

public class RouteService
{
    public FleetContext context;
    public RouteService(FleetContext _context)
    {
        context = _context;
    }

    public async Task<IEnumerable<Route>> GetAll() =>
        await context.Routes.ToListAsync();
    public async Task<Route?> Get(int id) =>
        await context.Routes.FirstOrDefaultAsync(route => route.Id == id);
    public async Task<bool> Add(Route route)
    {
        var addedRoute = context.Routes.Add(route);

        if (addedRoute == null)
            return false;

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Delete(int id)
    {
        var route = await context.Routes.FindAsync(id);

        if (route == null)
            return false;

        context.Routes.Remove(route);

        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> Update(Route route)
    {
        context.Routes.Update(route);

        return await context.SaveChangesAsync() > 0;
    }
}