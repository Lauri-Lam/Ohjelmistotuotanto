using MokkivarausApp.Services;

namespace MokkivarausApp;

public partial class CalendarPage : ContentPage
{
    private readonly DataService _dataService = new();

    public CalendarPage()
    {
        InitializeComponent();
        MonthPicker.DateSelected += MonthPicker_DateSelected;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMonthAsync(MonthPicker.Date);
    }

    private async void MonthPicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        await LoadMonthAsync(e.NewDate);
    }

    private async Task LoadMonthAsync(DateTime date)
    {
        var reservations = await _dataService
            .GetReservationsForMonthAsync(date.Year, date.Month);

        CalendarList.ItemsSource = reservations;
    }
}
