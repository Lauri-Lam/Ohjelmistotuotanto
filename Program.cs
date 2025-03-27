using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30))
    )
);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();
