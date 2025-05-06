using Microsoft.EntityFrameworkCore;
using Mökkivaraus.Models;

public class AppDbContext : DbContext
{
    public DbSet<Alue> Alueet => Set<Alue>();
    public DbSet<Mokki> Mokit => Set<Mokki>();
    public DbSet<Palvelu> Palvelut => Set<Palvelu>();
    public DbSet<Asiakas> Asiakkaat => Set<Asiakas>();
    public DbSet<Varaus> Varaukset => Set<Varaus>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}
