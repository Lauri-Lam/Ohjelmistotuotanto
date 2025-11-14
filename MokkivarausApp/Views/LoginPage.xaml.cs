using Microsoft.UI.Xaml.Media.Animation;
using MokkivarausApp.Data;
using MokkivarausApp.Models;
using MokkivarausApp.Services;

namespace MokkivarausApp;

public partial class LoginPage : ContentPage
{
    private DatabaseService databaseService = new();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Error", "Please enter your name.", "OK");
            return;
        }

        try
        {
            Asiakas asiakas = await databaseService.GetOrCreateCustomerByName(name);
            App.CurrentAsiakasId = asiakas.AsiakasId;

            Application.Current!.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database error", ex.Message, "OK");
        }
    }
}
