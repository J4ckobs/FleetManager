using FleetManager.Models;
using Route = FleetManager.Models.Route;

namespace FleetManager.Services;

public class RouteService
{
    public FileDataService<Route> routeService;
    public RouteService(FileDataService<Route> _routeService)
    {
        routeService = _routeService;
    }

    public async Task<IEnumerable<Route>> GetAll() => await routeService.GetAllAsync();
    public async Task<Route?> Get(int id) => await routeService.GetByPredictateAsync(x => x.Id == id);
    public async Task Add(Route route) => await routeService.AddAsync(route);
    public async Task Delete(int id) => await routeService.RemoveByPredictateAsync(x => x.Id == id);
    public async Task Update(Route route) => await routeService.UpdateAsync(x => x.Id == route.Id, route);
}