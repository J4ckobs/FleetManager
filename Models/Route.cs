

using Microsoft.VisualBasic;

namespace FleetManager.Models;

public class Route
{
    public int Id { get; set; }
    public string Departure { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public Vehicle? AssignedVehicle { get; set; }
    public Driver? AssignedDriver { get; set; }
    public float Distance { get; set; }
    public DateTime DepartureTime;
    public DateTime ArrivalTime;
}