using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Auth;
using DineTab_v1.Views.Cashier;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.Cashier
{
    public class CreateOrderViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService = new();

        public ObservableCollection<Item> MenuItems { get; set; } = new();
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        // Computed totals
        public decimal SubTotal => OrderItems.Sum(o => o.TotalPrice);
        public decimal Tax => SubTotal * 0.1m;
        public decimal Discount => 0m;
        public decimal Total => SubTotal + Tax - Discount;

        // Commands
        public ICommand AddToOrderCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand IncreaseOrderItemCommand { get; }
        public ICommand DecreaseOrderItemCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        private string _orderNumber;
        public string OrderNumber
        {
            get => _orderNumber;
            set
            {
                if (_orderNumber != value)
                {
                    _orderNumber = value;
                    OnPropertyChanged(nameof(OrderNumber));
                }
            }
        }

        private string _orderTypeText;
        public string OrderTypeText
        {
            get => _orderTypeText;
            set
            {
                if (_orderTypeText != value)
                {
                    _orderTypeText = value;
                    OnPropertyChanged(nameof(OrderTypeText));
                }
            }
        }
        public ICommand SelectCategoryCommand { get; }
        private ObservableCollection<Item> _allMenuItems = new(); // keep all items unfiltered


        public CreateOrderViewModel()
        {
            // Listen to changes in OrderItems to refresh totals
            OrderItems.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SubTotal));
                OnPropertyChanged(nameof(Tax));
                OnPropertyChanged(nameof(Total));
            };

            AddToOrderCommand = new Command<Item>(AddToOrder);
            RemoveItemCommand = new Command<OrderItem>(RemoveItem);
            CancelOrderCommand = new Command(CancelOrder);
            PlaceOrderCommand = new Command(PlaceOrder);

            // Commands for + / -
            IncreaseOrderItemCommand = new Command<OrderItem>(item =>
            {
                if (item != null)
                {
                    item.Quantity++;
                    RefreshTotals();
                }
            });

            DecreaseOrderItemCommand = new Command<OrderItem>(item =>
            {
                if (item != null && item.Quantity > 1)
                {
                    item.Quantity--;
                    RefreshTotals();
                }
            });

            SelectCategoryCommand = new Command<string>(cat =>
            {
                SelectedCategory = cat;
            });



            LoadMenuItems();
            LoadCategories();
        }

        private string _selectedCategory = "All Items"; // default
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged();
                ApplySearchAndCategoryFilter();   // filter when category changes
            }
        }

        private async void LoadMenuItems()
        {
            var itemsFromDb = await _databaseService.GetAvailableItemsAsync();
            MenuItems.Clear();

            foreach (var item in itemsFromDb)
            {
                MenuItems.Add(item);
                _allMenuItems.Add(item);
            }
        }

        private async void LoadCategories()
        {
            var categories = await _databaseService.GetCategoriesAsync();
            Categories.Clear();

            foreach (var cat in categories)
                Categories.Add(cat);
        }

        private void AddToOrder(Item item)
        {
            if (item == null) return;

            // Instead of navigating to AddOrderPage, just add it directly
            var existing = OrderItems.FirstOrDefault(o => o.ItemId == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                OrderItems.Add(new OrderItem
                {
                    ItemId = item.Id,
                    Name = item.ItemName,
                    Price = item.Price,
                    Quantity = 1
                });
            }

            RefreshTotals();
        }

        private void RemoveItem(OrderItem item)
        {
            if (item != null && OrderItems.Contains(item))
                OrderItems.Remove(item);

            RefreshTotals();
        }

        private async void CancelOrder()
        {
            
                OrderItems.Clear();
            try
            {
                Application.Current.MainPage = new NavigationPage(new CashierMenuPage());
            }
            catch (Exception ex) { }
               
        }

        private void RefreshTotals()
        {
            OnPropertyChanged(nameof(SubTotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
        }

        private async void PlaceOrder()
        {
            if (OrderItems.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No items in the order.", "OK");
                return;
            }

            OrderNumber = $"#{DateTime.Now:yyyyMMddHHmmss}";
            decimal totalAmount = Total; // calculated from OrderItems

            // Navigate to PaymentPage and pass order data
            await Application.Current.MainPage.Navigation.PushModalAsync(
                new PaymentPage(OrderNumber, totalAmount, OrderItems)
            );
        }
        private void ApplySearchAndCategoryFilter()
        {
            IEnumerable<Item> filtered = _allMenuItems;

            // Apply category filter (skip if "All Items")
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All Items")
            {
                filtered = filtered.Where(i => i.CategoryName.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }


            // Update UI collection
            MenuItems.Clear();
            foreach (var item in filtered)
                MenuItems.Add(item);
        }
    }

    // Converter to highlight only selected category
    public class CategoryToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = SelectedCategory from ViewModel
            // parameter = this button's category
            string selectedCategory = value as string;
            string thisCategory = parameter as string;

            if (selectedCategory == thisCategory)
                return Colors.Orange; // highlight selected

            return Colors.Transparent; // default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
