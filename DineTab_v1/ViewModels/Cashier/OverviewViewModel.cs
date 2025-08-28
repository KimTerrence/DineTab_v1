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
            public string MoneyReceived { get; }
            public string Change { get; }

            public ICommand CancelCommand { get; }
            public ICommand ConfirmCommand { get; }

            public OverviewViewModel(
                string orderNumber,
                ObservableCollection<OrderItem> items,
                string total,
                string moneyReceived,
                string change)
            {
                OrderNumber = orderNumber;
                OrderItems = items;
                TotalAmount = total;
                MoneyReceived = moneyReceived;
                Change = change;

                CancelCommand = new Command(OnCancel);
                ConfirmCommand = new Command(OnConfirm);
            }

            private async void OnCancel()
            {
                await Application.Current.MainPage.DisplayAlert("Cancelled", "Order was cancelled.", "OK");
                try
                {
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                catch { }
            }

            private async void OnConfirm()
            {
                try
                {
                    decimal total = decimal.Parse(TotalAmount);
                    decimal amountPaid = decimal.Parse(MoneyReceived);
                    decimal changeAmount = decimal.Parse(Change);

                    // Ensure order exists
                    bool orderExists = await _dbService.OrderExistsAsync(OrderNumber);
                    int orderId;

                    if (orderExists)
                    {
                        orderId = await _dbService.GetOrderIdByOrderNumberAsync(OrderNumber);
                        await _dbService.UpdateOrderPaymentStatusAsync(OrderNumber, "Paid");
                    }
                    else
                    {
                        orderId = await _dbService.InsertPaidOrderAsync(OrderNumber, "Dine In", total);
                        await _dbService.InsertOrderItemsAsync(orderId, OrderItems);
                    }

                    // Insert into Payments table
                    await _dbService.InsertPaymentAsync(
                        orderId,
                        amountPaid,
                        total,
                        changeAmount,
                        "Paid",
                        "Cash"
                    );


                // Navigate to receipt
                // Navigate to receipt
                await Application.Current.MainPage.Navigation.PushModalAsync(
                    new ReceiptPage(OrderNumber, OrderItems, total, amountPaid, changeAmount.ToString())
                );
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to confirm order: {ex.Message}", "OK");
                }
            }
        }
    }
