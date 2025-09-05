using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Services;


public class ReportsViewModel : INotifyPropertyChanged
{
    private readonly PdfService _pdfService = new();

    private readonly DatabaseService _dbService = new();

    public ObservableCollection<SoldItemReport> SoldItems { get; set; } = new();
    public ObservableCollection<SoldItemReport> FilteredSoldItems { get; set; } = new();

    public ObservableCollection<string> Categories { get; set; } = new();
    public ICommand ClearDateCommand { get; }

    public ICommand ExportPdfCommand => new Command(async () =>
    {
        var items = FilteredSoldItems.Select(x =>
            (x.OrderNo, x.TotalItem, x.TotalPrice, x.OrderDate)).ToList();

        var filePath = await _pdfService.CreateSalesReportAsync(
            items, TotalSoldItems, TotalOrders, TotalRevenue, FromDate, ToDate);

        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    });



    private string _selectedCategory = "All";
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                ApplyFilters();
            }
        }
    }

    private DateTime? _fromDate = null;
    public DateTime? FromDate
    {
        get => _fromDate;
        set
        {
            if (_fromDate != value)
            {
                _fromDate = value;
                OnPropertyChanged(nameof(FromDate));
                ApplyFilters();
            }
        }
    }

    private DateTime? _toDate = null;
    public DateTime? ToDate
    {
        get => _toDate;
        set
        {
            if (_toDate != value)
            {
                _toDate = value;
                OnPropertyChanged(nameof(ToDate));
                ApplyFilters();
            }
        }
    }


    private DateTime? _selectedDate = null;
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                ApplyFilters();
            }
        }
    }

    public ReportsViewModel()
    {
        LoadSoldItems();

        ClearDateCommand = new Command(() =>
        {
            FromDate = null;
            ToDate = null;
            ApplyFilters();
        });

    }

    private async void LoadSoldItems()
    {
        var items = await _dbService.GetSoldItemsAsync();
        SoldItems.Clear();
        foreach (var item in items)
            SoldItems.Add(item);

        // Load categories (distinct from items)
        Categories.Clear();
        Categories.Add("All");
        foreach (var cat in SoldItems.Select(s => s.Type).Distinct())
            Categories.Add(cat);

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var query = SoldItems.AsEnumerable();

        // Filter by category
        if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            query = query.Where(s => s.Type == SelectedCategory);

        // Filter by date range
        if (FromDate.HasValue && ToDate.HasValue)
            query = query.Where(s => s.OrderDate.Date >= FromDate.Value.Date &&
                                     s.OrderDate.Date <= ToDate.Value.Date);
        else if (FromDate.HasValue) // Only from date selected
            query = query.Where(s => s.OrderDate.Date >= FromDate.Value.Date);
        else if (ToDate.HasValue) // Only to date selected
            query = query.Where(s => s.OrderDate.Date <= ToDate.Value.Date);

        // Apply results
        FilteredSoldItems.Clear();
        foreach (var item in query)
            FilteredSoldItems.Add(item);

        // Update totals
        OnPropertyChanged(nameof(TotalSoldItems));
        OnPropertyChanged(nameof(TotalRevenue));
        OnPropertyChanged(nameof(TotalOrders));
        OnPropertyChanged(nameof(OrdersToday));
    }


    // Totals based on FilteredSoldItems
    public int TotalSoldItems => FilteredSoldItems.Sum(s => s.TotalItem);
    public int OrdersToday => FilteredSoldItems.Count(s => s.OrderDate.Date == DateTime.Today);
    public decimal TotalRevenue => FilteredSoldItems.Sum(s => s.TotalPrice);
    public int TotalOrders => FilteredSoldItems.Count;

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
