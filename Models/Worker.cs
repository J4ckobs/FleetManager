
using System.ComponentModel.DataAnnotations;

namespace FleetManager.Models;

public class Worker : IEntity
{
    [Required]
    public int Id { get; set; }
}