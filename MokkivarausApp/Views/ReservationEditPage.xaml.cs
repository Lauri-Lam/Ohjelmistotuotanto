using MokkivarausApp.Data;
using MokkivarausApp.Models;
using MokkivarausApp.Services;

namespace MokkivarausApp;

[QueryProperty(nameof(Reservation), "Reservation")]
public partial class ReservationEditPage : ContentPage
{
    private readonly DataService _dataService = new();
    private DatabaseService databaseService = new();
    private List<Mokki> _cabins = new();
    private Varaus _reservation;

    public Varaus Reservation
    {
        get => _reservation;
        set
        {
            _reservation = value;
            // UI is filled in OnAppearing after cabins are loaded
        }
    }

    public ReservationEditPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCabinsAsync();
    }

    private async Task LoadCabinsAsync()
    {
        _cabins = await databaseService.GetAllCabinsAsync();
        CabinPicker.ItemsSource = _cabins;
        CabinPicker.ItemDisplayBinding = new Binding(nameof(Mokki.Mokkinimi));

        if (_cabins.Count > 0 && CabinPicker.SelectedIndex < 0)
            CabinPicker.SelectedIndex = 0;

        if (_reservation != null)
        {
            var index = _cabins.FindIndex(c => c.MokkiId == _reservation.MokkiId);
            if (index >= 0)
                CabinPicker.SelectedIndex = index;

            if (_reservation.VarattuAlkuPvm.HasValue)
                StartDatePicker.Date = _reservation.VarattuAlkuPvm.Value.Date;

            if (_reservation.VarattuLoppuPvm.HasValue)
                EndDatePicker.Date = _reservation.VarattuLoppuPvm.Value.Date;
        }
        else
        {
            StartDatePicker.Date = DateTime.Today;
            EndDatePicker.Date = DateTime.Today.AddDays(1);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (CabinPicker.SelectedItem is not Mokki selectedCabin)
        {
            await DisplayAlert("Error", "Select a cabin.", "OK");
            return;
        }

        var start = StartDatePicker.Date;
        var end = EndDatePicker.Date;

        if (end <= start)
        {
            await DisplayAlert("Error", "Departure must be after arrival.", "OK");
            return;
        }

        try
        {
            if (_reservation == null)
            {
                await databaseService.CreateReservationAsync(
                    App.CurrentAsiakasId,
                    selectedCabin.MokkiId,
                    start,
                    end);
            }
            else
            {
                await databaseService.UpdateReservationAsync(
                    _reservation.VarausId,
                    selectedCabin.MokkiId,
                    start,
                    end);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database error", ex.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_reservation == null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        bool confirm = await DisplayAlert("Delete", "Delete this reservation?", "Yes", "No");
        if (!confirm)
            return;

        try
        {
            await _dataService.DeleteReservationAsync(_reservation.VarausId);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database error", ex.Message, "OK");
        }
    }
}
