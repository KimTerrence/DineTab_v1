using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Views.Cashier;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Cashier
{
    public class OverviewViewModel : BaseViewModel
    {

        private readonly DatabaseService _dbService = new DatabaseService();
        public string OrderNumber { get; }
        public ObservableCollection<OrderItem> OrderItems { get; }
        public string TotalAmount { get; }

        public ICommand CancelCommand { get; }
        public ICommand ConfirmCommand { get; }

        public OverviewViewModel(string orderNumber, ObservableCollection<OrderItem> items, string total)
        {

            OrderNumber = orderNumber;
            OrderItems = items;
            TotalAmount = total;

            CancelCommand = new Command(OnCancel);
            ConfirmCommand = new Command(OnConfirm);

        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.DisplayAlert("Cancelled", "Order was cancelled.", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }


        private async void OnConfirm()
        {
            try
            {
                // Convert TotalAmount string to decimal
                decimal total = decimal.Parse(TotalAmount);

                // Check if order exists in MS SQL
                bool orderExists = await _dbService.OrderExistsAsync(OrderNumber);

                if (orderExists)
                {
                    // If order exists, mark as Paid
                    await _dbService.UpdateOrderPaymentStatusAsync(OrderNumber, "Paid");
                }
                else
                {
                    // If order does not exist, insert the order
                    int orderId = await _dbService.InsertPaidOrderAsync(OrderNumber, "Dine In", total); // Replace "Dine In" with actual order type if needed

                    // Insert order items
                    await _dbService.InsertOrderItemsAsync(orderId, OrderItems);
                }

                // Navigate to ReceiptPage
                await Application.Current.MainPage.Navigation.PushModalAsync(
                    new ReceiptPage(OrderNumber, OrderItems, total)
                );
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to confirm order: {ex.Message}", "OK");
            }
        }


    }
}

