namespace MokkivarausApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ReservationEditPage), typeof(ReservationEditPage));
        Routing.RegisterRoute(nameof(CabinEditPage), typeof(CabinEditPage));
    }
}
