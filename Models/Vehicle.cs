using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Vehicle
{
    [Required]
    public int Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
    [Required]
    public string? LicensePlate { get; set; } = null;
    public VehicleStatus Status { get; set; }
    public int AverageSpeed { get; set; }
}

public enum VehicleStatus
{
    Available,
    Assigned,
    In_Service
};