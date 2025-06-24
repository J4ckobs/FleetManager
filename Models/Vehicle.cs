using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Vehicle : IEntity
{
    [Required]
    public int Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
    public string? LicensePlate { get; set; }
    public float AverageSpeed { get; set; }
    public VehicleStatus Status { get; set; }
    public int RouteId { get; set; }
}