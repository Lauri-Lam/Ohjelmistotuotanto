using MokkivarausApp.Models;
using MokkivarausApp.Services;
using MySql.Data.MySqlClient;

namespace MokkivarausApp;

[QueryProperty(nameof(Cabin), "Cabin")]
public partial class CabinEditPage : ContentPage
{
    private readonly DataService _dataService = new();
    private Mokki _cabin;

    public Mokki Cabin
    {
        get => _cabin;
        set
        {
            _cabin = value;
            if (_cabin != null)
            {
                NameEntry.Text = _cabin.Mokkinimi;
                StreetEntry.Text = _cabin.Katuosoite;
                PostEntry.Text = _cabin.Postinro;
                AreaEntry.Text = _cabin.AlueId.ToString();
                PriceEntry.Text = _cabin.Hinta.ToString("0.00");
            }
        }
    }

    public CabinEditPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!uint.TryParse(AreaEntry.Text, out var alueId) ||
            string.IsNullOrWhiteSpace(PostEntry.Text) ||
            !double.TryParse(PriceEntry.Text, out var hinta))
        {
            await DisplayAlert("Error", "Fill Area ID, Postal code and Price.", "OK");
            return;
        }

        var mokki = _cabin ?? new Mokki();

        mokki.AlueId = alueId;
        mokki.Postinro = PostEntry.Text.Trim();
        mokki.Mokkinimi = NameEntry.Text?.Trim();
        mokki.Katuosoite = StreetEntry.Text?.Trim();
        mokki.Hinta = hinta;

        try
        {
            if (_cabin == null || _cabin.MokkiId == 0)
            {
                await _dataService.CreateMokkiAsync(mokki);
            }
            else
            {
                mokki.MokkiId = _cabin.MokkiId;
                await _dataService.UpdateMokkiAsync(mokki);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (MySqlException ex) when (ex.Number == 1452)
        {
            await DisplayAlert("Error",
                "Area ID or postal code does not exist in the database. " +
                "Use an existing Area ID or add a new area/postal code.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database error", ex.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_cabin == null || _cabin.MokkiId == 0)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        bool confirm = await DisplayAlert("Delete cabin", "Delete this cabin?", "Yes", "No");
        if (!confirm)
            return;

        try
        {
            await _dataService.DeleteMokkiAsync(_cabin.MokkiId);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database error", ex.Message, "OK");
        }
    }
}
