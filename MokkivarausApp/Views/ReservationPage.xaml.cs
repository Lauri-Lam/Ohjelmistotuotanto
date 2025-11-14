using MokkivarausApp.Data;
using MokkivarausApp.Models;
using MokkivarausApp.Services;

namespace MokkivarausApp;

public partial class ReservationPage : ContentPage
{
    private DatabaseService databaseService = new();

    public ReservationPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReservationsAsync();
    }

    private async Task LoadReservationsAsync()
    {
        if (App.CurrentAsiakasId == 0)
            return;

        var reservations = await databaseService.GetReservationsByCustomerIDAsync(App.CurrentAsiakasId);

        ReservationList.ItemsSource = reservations;
    }

    private async void OnNewReservationClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReservationEditPage));
    }

    private async void OnReservationSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Varaus selected)
        {
            await Shell.Current.GoToAsync(nameof(ReservationEditPage),
                new Dictionary<string, object>
                {
                    ["Reservation"] = selected
                });
        }

        ((ListView)sender).SelectedItem = null;
    }
}
