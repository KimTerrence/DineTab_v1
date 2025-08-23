using DineTab_v1.Models;
using DineTab_v1.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.Cashier
{
    using System.Linq;

    public class CashierMenuViewModel : BindableObject
    {
        private readonly DatabaseService _dbService = new();

        public ObservableCollection<OrderDisplay> Orders { get; set; } = new();
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();

        public ICommand AddOrderCommand { get; }
        public ICommand IncreaseOrderItemCommand { get; }
        public ICommand DecreaseOrderItemCommand { get; }

        public CashierMenuViewModel()
        {
            LoadOrders();
            AddOrderCommand = new Command<OrderDisplay>(AddOrder);
            IncreaseOrderItemCommand = new Command<OrderItem>(IncreaseQuantity);
            DecreaseOrderItemCommand = new Command<OrderItem>(DecreaseQuantity);
        }

        private async void LoadOrders()
        {
            Orders.Clear();
            var ordersFromDb = await _dbService.GetOrdersAsync();
            foreach (var order in ordersFromDb)
            {
                var items = await _dbService.GetOrderItemsAsync(order.OrderId);
                Orders.Add(new OrderDisplay
                {
                    OrderNumber = order.OrderNumber,
                    OrderType = order.OrderType,
                    Items = items
                });
            }
        }

        private string _selectedOrderNumber;
        public string SelectedOrderNumber
        {
            get => _selectedOrderNumber;
            set { _selectedOrderNumber = value; OnPropertyChanged(); }
        }

        private string _selectedOrderType;
        public string SelectedOrderType
        {
            get => _selectedOrderType;
            set { _selectedOrderType = value; OnPropertyChanged(); }
        }

        public decimal Total => OrderItems.Sum(i => i.TotalPrice);

        private void AddOrder(OrderDisplay orderDisplay)
        {
            if (orderDisplay == null) return;

            OrderItems.Clear();
            foreach (var item in orderDisplay.Items)
                OrderItems.Add(item);

            SelectedOrderNumber = orderDisplay.OrderNumber;
            SelectedOrderType = orderDisplay.OrderType;

            OnPropertyChanged(nameof(OrderItems));
            OnPropertyChanged(nameof(SelectedOrderNumber));
            OnPropertyChanged(nameof(SelectedOrderType));
            OnPropertyChanged(nameof(Total));
        }

        private void IncreaseQuantity(OrderItem item)
        {
            if (item == null) return;
            item.Quantity += 1;
            OnPropertyChanged(nameof(Total));
        }

        private void DecreaseQuantity(OrderItem item)
        {
            if (item == null || item.Quantity <= 1) return;
            item.Quantity -= 1;
            OnPropertyChanged(nameof(Total));
        }
    }
}