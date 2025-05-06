using Microsoft.EntityFrameworkCore;
using MokkivarausApp.Data;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { /*…*/ });

        // TODO: ota talteen oikea connection string
        var connStr = "Server=HOST;Port=3306;Database=Mokkivaraus;User=USER;Password=PWD;";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

        // rekisteröi viewmodelit
        builder.Services.AddTransient<AlueetViewModel>();
        builder.Services.AddTransient<MokitViewModel>();
        // … sama muille

        return builder.Build();
    }
}
