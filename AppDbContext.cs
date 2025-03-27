using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Alue> Alueet { get; set; }
        public DbSet<Mokki> Mokit { get; set; }
        public DbSet<Palvelu> Palvelut { get; set; }
        public DbSet<Asiakas> Asiakkaat { get; set; }
        public DbSet<Varaus> Varaukset { get; set; }
        public DbSet<Lasku> Laskut { get; set; }
    }
}
