using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Driver : IEntity
{
    [Required]
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateOnly LicenseExpiryDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int? RouteId { get; set; } = -1;
}