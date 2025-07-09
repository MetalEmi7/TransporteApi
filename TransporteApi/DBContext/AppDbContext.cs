using global::TransporteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TransporteAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

    }
}
