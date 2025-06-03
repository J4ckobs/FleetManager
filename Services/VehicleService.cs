using FleetManager.Models;

namespace FleetManager.Services;

public static class VehicleService
{
    public static List<Vehicle> Vehicles { get; set; }

    public static int Index = 4;

    static VehicleService()
    {
        Vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, Brand = "Toyota", Model = "Corolla", Mileage = 150345, Year = 2006, Status = "Garage"},
            new Vehicle { Id = 2, Brand = "Ford", Model = "Focus", Mileage = 121450, Year = 2013, Status = "Garage"},
            new Vehicle { Id = 3, Brand = "BWM", Model = "M3", Mileage = 67781, Year = 2017, Status = "Garage"}
        };
    }

    public static IEnumerable<Vehicle> GetAll() => Vehicles;

    public static Vehicle? Get(int id) => Vehicles.FirstOrDefault(x => x.Id == id);

    public static void Add(Vehicle vehicle)
    {
        Index++;
        Vehicles.Add(vehicle);
    }

    public static void Delete(int id)
    {
        var vehicle = Get(id);

        if (vehicle == null)
            return;

        Vehicles.Remove(vehicle);
    }

    public static void Update(Vehicle vehicle)
    {
        var index = Vehicles.FindIndex(x => x.Id == vehicle.Id);

        if (index == -1)
            return;

        Vehicles[index] = vehicle;
    }
}