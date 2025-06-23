
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Route : IEntity
{
    [Required]
    public int Id { get; set; }
    public string? Departure { get; set; }
    public string? Destination { get; set; }
    public string? DepartureCoords { get; set; }
    public string? DestinationCoords { get; set; }
    public int AssignedVehicle { get; set; } = -1;
    public int AssignedDriver { get; set; } = -1;
    public float Distance { get; set; } = -1;
    public DateTime DepartureTime;
    public DateTime ArrivalTime;
}