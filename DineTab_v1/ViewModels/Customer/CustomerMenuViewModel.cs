using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Services;
using Microsoft.Maui.Controls;

namespace DineTab_v1.ViewModels.Customer
{
    public class CustomerMenuViewModel : BindableObject
    {
        private readonly DatabaseService databaseService = new();

        public ObservableCollection<Item> MenuItems { get; set; } = new();
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();

        public ICommand AddToOrderCommand { get; }

        public CustomerMenuViewModel()
        {
            AddToOrderCommand = new Command<Item>(AddToOrder);

            // Load items from database
            LoadMenuItems();
        }

        private async void LoadMenuItems()
        {
            var itemsFromDb = await databaseService.GetMenuItemsAsync();
            foreach (var item in itemsFromDb)
            {
                if (item.Availability.ToLower() == "available") // Only available items
                    MenuItems.Add(item);
            }
        }

        private void AddToOrder(Item item)
        {
            var existing = OrderItems.FirstOrDefault(o => o.Name == item.ItemName);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                OrderItems.Add(new OrderItem
                {
                    Name = item.ItemName,
                    UnitPrice = item.Price,
                    Quantity = 1
                });
            }

            OnPropertyChanged(nameof(SubTotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
        }

        public decimal SubTotal => OrderItems.Sum(o => o.TotalPrice);
        public decimal Tax => SubTotal * 0.1m;
        public decimal Discount => 0m;
        public decimal Total => SubTotal + Tax - Discount;
    }
}
