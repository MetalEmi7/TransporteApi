//namespace TransporteApi.DBContext

using global::TransporteAPI.Models;
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
//using TransporteAPI.Models;

namespace TransporteAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();


    }
}
