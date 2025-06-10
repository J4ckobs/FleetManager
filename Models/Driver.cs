using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Driver
{
    [Required]
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateTime LicenseExpiryDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? AssignedVehicleLicensePlate { get; set; } = null;
}