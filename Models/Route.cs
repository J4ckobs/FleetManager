

using Microsoft.VisualBasic;

namespace FleetManager.Models;

public class Route
{
    public int Id { get; set; }
    public string Departure { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public Vehicle? AssignedVehicle { get; set; } = null;
    public Driver? AssignedDriver { get; set; } = null;
    public float Distance { get; set; } = -1;
    public DateTime DepartureTime;
    public DateTime ArrivalTime;
}