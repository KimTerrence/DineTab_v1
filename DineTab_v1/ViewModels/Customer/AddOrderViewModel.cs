using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DineTab_v1.Models;

namespace DineTab_v1.ViewModels.Customer
{
    public class AddOrderViewModel : BindableObject
    {
        public ObservableCollection<OrderItem> OrderItems { get; }
        public Item SelectedItem { get; set; }

        private int quantity = 1;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public decimal TotalPrice => (SelectedItem?.Price ?? 0) * Quantity;

        // ✅ New summary properties
        private decimal subTotal;
        public decimal SubTotal
        {
            get => subTotal;
            set { subTotal = value; OnPropertyChanged(); }
        }

        private decimal tax;
        public decimal Tax
        {
            get => tax;
            set { tax = value; OnPropertyChanged(); }
        }

        private decimal discount;
        public decimal Discount
        {
            get => discount;
            set { discount = value; OnPropertyChanged(); }
        }

        private decimal total;
        public decimal Total
        {
            get => total;
            set { total = value; OnPropertyChanged(); }
        }

        public Command ConfirmAddCommand { get; }
        public Command CancelCommand { get; }

        public Command IncreaseQuantityCommand { get; }
        public Command DecreaseQuantityCommand { get; }

        public AddOrderViewModel(ObservableCollection<OrderItem> orderItems)
        {
            OrderItems = orderItems;
            OrderItems.CollectionChanged += OrderItems_CollectionChanged;

            ConfirmAddCommand = new Command(AddToOrder);
            CancelCommand = new Command(Close);

            IncreaseQuantityCommand = new Command(() =>
            {
                Quantity++;
                RecalculateTotals();
            });

            DecreaseQuantityCommand = new Command(() =>
            {
                if (Quantity > 1)
                    Quantity--;
                RecalculateTotals();
            });


            RecalculateTotals();
        }

        private void OrderItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecalculateTotals();
        }

        private async void AddToOrder()
        {
            if (SelectedItem == null)
                return;

            // Check if item already exists in the order
            var existingItem = OrderItems.FirstOrDefault(i => i.ItemId == SelectedItem.Id);

            if (existingItem != null)
            {
                // If exists, just increase the quantity
                existingItem.Quantity += Quantity;
                OnPropertyChanged(nameof(OrderItems));
            }
            else
            {
                // Otherwise, add as new
                var orderItem = new OrderItem
                {
                    ItemId = SelectedItem.Id,
                    Name = SelectedItem.ItemName,
                    Price = SelectedItem.Price,
                    Quantity = Quantity
                };

                OrderItems.Add(orderItem);
            }

            // Close the modal/page
            if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
                await Application.Current.MainPage.Navigation.PopModalAsync();
            else
                await Application.Current.MainPage.Navigation.PopAsync();
        }


        private void RecalculateTotals()
        {
            SubTotal = OrderItems.Sum(i => i.TotalPrice);

            Tax = SubTotal * 0.12m;      // Example: 12% VAT
            Discount = SubTotal * 0.05m; // Example: 5% discount
            Total = SubTotal + Tax - Discount;
        }

        public async void Close()
        {
            if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
                await Application.Current.MainPage.Navigation.PopModalAsync();
            else
                await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}