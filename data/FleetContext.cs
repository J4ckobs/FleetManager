using Microsoft.EntityFrameworkCore;

namespace FleetManager.Models
{
    public class FleetContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Route> Routes { get; set; }

        public FleetContext(DbContextOptions options) : base(options)
        { }
    }
}