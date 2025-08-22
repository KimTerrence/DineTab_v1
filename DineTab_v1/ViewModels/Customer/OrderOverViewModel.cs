using DineTab_v1.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Customer
{
    public class OrderOverviewViewModel
    {
        private readonly DatabaseService _dbService;
        public ObservableCollection<OrderItem> OrderItems { get; }
        public decimal Total { get; }
        public string OrderNumber { get; }
        public string OrderType { get; }

        // Commands
        public ICommand CancelCommand { get; }
        public ICommand ConfirmCommand { get; }


        public OrderOverviewViewModel(
         DatabaseService dbService,
         ObservableCollection<OrderItem> orderItems,
         decimal total,
         string orderNumber,
         string orderType)

        {
            _dbService = dbService;
            OrderItems = orderItems;
            Total = total;
            OrderNumber = orderNumber;
            OrderType = orderType;

            CancelCommand = new Command(OnCancel);
            ConfirmCommand = new Command(OnConfirm);
        }


        private async void OnCancel()
        {
            // Close the popup / modal or navigate back
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception ex) { }
            
        }

        private async void OnConfirm()
        {
            try
            {
                // Insert Order and get orderId
                int orderId = await _dbService.InsertOrderAsync(OrderNumber, OrderType, Total);

                // Insert Items using orderId
                await _dbService.InsertOrderItemsAsync(orderId, OrderItems);

                // Navigate to ThankYouPage after saving order
                await Application.Current.MainPage.Navigation.PushModalAsync(new Views.Customer.ThankYouPage());

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save order: {ex.Message}", "OK");
            }
        }
    }
}
