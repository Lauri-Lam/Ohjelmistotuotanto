using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MokkivarausApp.Data;
using MokkivarausApp.Models;

public partial class AlueetViewModel : ObservableObject
{
    private readonly AppDbContext _db;

    public ObservableCollection<Alue> Alueet { get; } = new();

    [ObservableProperty]
    private string _uusiAlueNimi;

    public AlueetViewModel(AppDbContext db)
    {
        _db = db;
        LoadCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Alueet.Clear();
        var list = await _db.Alueet.ToListAsync();
        foreach (var a in list)
            Alueet.Add(a);
    }

    [RelayCommand]
    public async Task AddAsync()
    {
        var a = new Alue { Nimi = UusiAlueNimi };
        _db.Alueet.Add(a);
        await _db.SaveChangesAsync();
        Alueet.Add(a);
        UusiAlueNimi = string.Empty;
    }

    [RelayCommand]
    public async Task DeleteAsync(Alue alue)
    {
        _db.Alueet.Remove(alue);
        await _db.SaveChangesAsync();
        Alueet.Remove(alue);
    }

    // Muokkaus ja haku voitaisiin toteuttaa vastaavasti…
}
