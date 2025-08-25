using System.ComponentModel;
using System.Threading.Tasks;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Admin
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService = new();

        private string _dailyRevenue;
        private string _activeOrders;
        private string _completedOrders;
        private string _staffStatus;
        private string _dineIN;
        private string _takeOut;
        private string _pending;
        private string _preparing;
        private string _ready;

        public string DailyRevenue
        {
            get => _dailyRevenue;
            set { _dailyRevenue = value; OnPropertyChanged(nameof(DailyRevenue)); }
        }

        public string ActiveOrders
        {
            get => _activeOrders;
            set { _activeOrders = value; OnPropertyChanged(nameof(ActiveOrders)); }
        }

        public string CompletedOrders
        {
            get => _completedOrders;
            set { _completedOrders = value; OnPropertyChanged(nameof(CompletedOrders)); }
        }

        public string StaffStatus
        {
            get => _staffStatus;
            set { _staffStatus = value; OnPropertyChanged(nameof(StaffStatus)); }
        }

        public string DineIn
        {
            get => _dineIN;
            set { _dineIN = value; OnPropertyChanged(nameof(DineIn)); }
        }

        public string TakeOut
        {
            get => _takeOut;
            set { _takeOut = value; OnPropertyChanged(nameof(TakeOut)); }
        }

        public string Pending
        {
            get => _pending;
            set { _pending = value; OnPropertyChanged(nameof(Pending)); }
        }

        public string Preparing
        {
            get => _preparing;
            set { _preparing = value; OnPropertyChanged(nameof(Preparing)); }
        }

        public string Ready
        {
            get => _ready;
            set { _ready = value; OnPropertyChanged(nameof(Ready)); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardViewModel()
        {
            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            DailyRevenue = (await _dbService.GetDailyRevenueAsync()).ToString("F2");
            ActiveOrders = (await _dbService.GetActiveOrdersAsync()).ToString();
            CompletedOrders = (await _dbService.CountCompletedOrdersAsync()).ToString();
            StaffStatus = (await _dbService.GetActiveStaffAsync()).ToString();
            DineIn = (await _dbService.GetDineIn()).ToString();
            TakeOut = (await _dbService.GetTakeOut()).ToString();
            Pending = (await _dbService.GetPending()).ToString();
            Preparing = (await _dbService.GetPreparing()).ToString();
            Ready = (await _dbService.GetReady()).ToString();

        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
