namespace MokkivarausApp;

public partial class App : Application
{
    // current logged-in customer id
    public static uint CurrentAsiakasId { get; set; }

    public App()
    {
        InitializeComponent();

        // Start from login page
        MainPage = new NavigationPage(new LoginPage());
    }
}
