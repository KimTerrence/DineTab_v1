using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Views.Cashier;

namespace DineTab_v1.ViewModels.Cashier
{
    public class OverviewViewModel : BaseViewModel
    {
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
            // Convert TotalAmount string to decimal
            decimal total = decimal.Parse(TotalAmount);

            // Pass arguments to ReceiptPage
            await Application.Current.MainPage.Navigation.PushModalAsync(
                new ReceiptPage(OrderNumber, OrderItems, total)
            );
        }

    }
}

