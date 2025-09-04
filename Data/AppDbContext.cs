using Microsoft.EntityFrameworkCore;
using ReservasSalas.Models;

namespace ReservasSalas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reserva> Reservas { get; set; }
    }
}
