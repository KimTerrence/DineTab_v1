using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DineTab_v1.Models;
using DineTab_v1.Services;

public class ReportsViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _dbService = new();

    public ObservableCollection<SoldItemReport> SoldItems { get; set; } = new();

    public ReportsViewModel()
    {
        LoadSoldItems();
    }

    private async void LoadSoldItems()
    {
        var items = await _dbService.GetSoldItemsAsync();
        SoldItems.Clear();
        foreach (var item in items)
            SoldItems.Add(item);

        // Raise property changed for totals
        OnPropertyChanged(nameof(TotalSoldItems));
        OnPropertyChanged(nameof(TotalRevenue));
        OnPropertyChanged(nameof(TotalOrders));
    }

    // Total number of items sold
    public int TotalSoldItems => SoldItems.Sum(s => s.TotalItem);

    // Total revenue
    public decimal TotalRevenue => SoldItems.Sum(s => s.TotalPrice);

    // Total orders (today or overall)
    public int TotalOrders => SoldItems.Count;

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
