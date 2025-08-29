using DineTab_v1.Models;
using DineTab_v1.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DineTab_v1.Views.Cashier;
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
        public ICommand ConfirmPaymentCommand { get; }
        public ICommand CreateOrderCommand {  get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand SignOutCommand { get; }

        public CashierMenuViewModel()
        {
            LoadOrders();
            AddOrderCommand = new Command<OrderDisplay>(AddOrder);
            IncreaseOrderItemCommand = new Command<OrderItem>(IncreaseQuantity);
            ConfirmPaymentCommand = new Command(OnConfirmPayment);
            DecreaseOrderItemCommand = new Command<OrderItem>(DecreaseQuantity);
            CreateOrderCommand = new Command(CreateOrder);
            CancelOrderCommand = new Command(CancelOrder);
            SignOutCommand = new Command(async () =>
            {
                bool confirmed = await Application.Current.MainPage.DisplayAlert(
                    "Sign Out", "Are you sure you want to sign out?", "Yes", "No");

                if (confirmed)
                    Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
            });
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
                    OrderId = order.OrderId,
                    OrderNumber = order.OrderNumber,
                    OrderType = order.OrderType,
                    Items = items
                });
            }
        }
        private int _selectedOrderId;
        public int SelectedOrderId
        {
            get => _selectedOrderId;
            set { _selectedOrderId = value; OnPropertyChanged(); }
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

            SelectedOrderId = orderDisplay.OrderId;
            SelectedOrderNumber = orderDisplay.OrderNumber;
            SelectedOrderType = orderDisplay.OrderType;

            OnPropertyChanged(nameof(OrderItems));
            OnPropertyChanged(nameof(SelectedOrderId));
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
        private async void OnConfirmPayment()
        {
            try
            {
                var orderNumber = SelectedOrderNumber;
            var totalAmount = Total;
            var items = new ObservableCollection<OrderItem>(OrderItems);
           
               await Shell.Current.Navigation.PushModalAsync(
               new PaymentPage(orderNumber, totalAmount, items)
           );


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }   
        }

        public async void CreateOrder()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new CreateOrderPage());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
          
        }
        private async void CancelOrder()
        {
            if (SelectedOrderId == 0) return; // No order selected

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Cancel Order",
                "Are you sure you want to cancel this order?",
                "Yes", "No");

            if (!confirm) return;

            // Update status in database
            bool success = await _dbService.UpdateOrderStatusAsync(SelectedOrderId, "Canceled");

            if (success)
            {
                // Clear current order in UI
                OrderItems.Clear();
                SelectedOrderId = 0;
                SelectedOrderNumber = string.Empty;
                SelectedOrderType = string.Empty;
                OnPropertyChanged(nameof(Total));

                // Optionally remove from Orders collection
                var orderToRemove = Orders.FirstOrDefault(o => o.OrderId == SelectedOrderId);
                if (orderToRemove != null)
                    Orders.Remove(orderToRemove);

                LoadOrders();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to cancel order.", "OK");
            }
        }
    }
}