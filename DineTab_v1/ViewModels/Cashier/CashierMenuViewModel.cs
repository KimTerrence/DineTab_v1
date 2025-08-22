using DineTab_v1.Models;
using DineTab_v1.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DineTab_v1.ViewModels.Cashier
{
    public class CashierMenuViewModel : BindableObject
    {
        private readonly DatabaseService _dbService = new();

        public ObservableCollection<OrderDisplay> Orders { get; set; } = new();

        public CashierMenuViewModel()
        {
            LoadOrders();
        }

        private async void LoadOrders()
        {
            Orders.Clear();

            var ordersFromDb = await _dbService.GetOrdersAsync(); // returns List<Order>
            foreach (var order in ordersFromDb)
            {
                var items = await _dbService.GetOrderItemsAsync(order.OrderId); // returns ObservableCollection<OrderItem>
                Orders.Add(new OrderDisplay
                {
                    OrderNumber = order.OrderNumber,
                    OrderType = order.OrderType,
                    Items = items
                });
            }
        }
    }
}
