

namespace FleetManager.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string? Status { get; set; }
}