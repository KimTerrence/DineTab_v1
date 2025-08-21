using System.Collections.ObjectModel;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Customer;

namespace DineTab_v1.ViewModels.Customer
{
    public class CustomerMenuViewModel : BindableObject
    {
        private readonly DatabaseService databaseService = new();

        public ObservableCollection<Item> MenuItems { get; set; } = new();
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        // Child overlay viewmodel
        public AddOrderViewModel AddOrderVM { get; }

        public ICommand AddToOrderCommand { get; }
        public ICommand CategorySelectedCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public CustomerMenuViewModel()
        {
            AddOrderVM = new AddOrderViewModel(OrderItems); // pass in shared OrderItems

            AddToOrderCommand = new Command<Item>(AddToOrder);
            SignOutCommand = new Command(SignOut);
            RemoveItemCommand = new Command<OrderItem>(RemoveItem);
            LoadMenuItems();
            LoadCategories();
        }

        private async void LoadMenuItems()
        {
            var itemsFromDb = await databaseService.GetMenuItemsAsync();
            foreach (var item in itemsFromDb)
            {
                if (item.Availability.ToLower() == "available")
                    MenuItems.Add(item);
            }
        }

        private async void  AddToOrder(Item item)
        {
            if (item == null) return;
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddOrderPage(item, OrderItems));
        }

        private async void LoadCategories()
        {
            var categories = await databaseService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }
        }

        public async void SignOut()
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert(
                "Go Back", "Are you sure you want to go back?", "Yes", "No");

            if (confirmed)
                Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
        }

        private void RemoveItem(OrderItem item)
        {
            if (item != null && OrderItems.Contains(item))
            {
                OrderItems.Remove(item);
            }
        }
        public decimal SubTotal => OrderItems.Sum(o => o.TotalPrice);
        public decimal Tax => SubTotal * 0.1m;
        public decimal Discount => 0m;
        public decimal Total => SubTotal + Tax - Discount;
    }
}