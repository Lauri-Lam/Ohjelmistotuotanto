using MokkivarausApp.Models;
using MokkivarausApp.Services;

namespace MokkivarausApp;

public partial class AdminCabinsPage : ContentPage
{
    private readonly DataService _dataService = new();

    public AdminCabinsPage()
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
        var cabins = await _dataService.GetAllMokitAsync();
        CabinList.ItemsSource = cabins;
    }

    private async void OnAddCabinClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CabinEditPage));
    }

    private async void OnCabinSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Mokki selected)
        {
            await Shell.Current.GoToAsync(nameof(CabinEditPage),
                new Dictionary<string, object>
                {
                    ["Cabin"] = selected
                });
        }

        ((ListView)sender).SelectedItem = null;
    }
}
