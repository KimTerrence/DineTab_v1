using DineTab_v1.Models;
using DineTab_v1.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace DineTab_v1.ViewModels.KitchenStaff
{
    public class HistoryViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Order> TodayOrders { get; set; } = new();
        public ObservableCollection<Order> YesterdayOrders { get; set; } = new();

        public ICommand RecallCommand { get; }
        public ICommand GobackCommand { get; }

        public HistoryViewModel()
        {
            _databaseService = new DatabaseService();

            RecallCommand = new Command<Order>(RecallOrder);

            GobackCommand = new Command( async () =>
            {
                try
                {
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                catch (Exception ex) { }
              
            });

            LoadCompletedOrders();
        }

        private async void LoadCompletedOrders()
        {
            TodayOrders.Clear();
            YesterdayOrders.Clear();

            // Get all completed orders from database
            var completedOrders = await _databaseService.GetAllCompletedOrdersAsync();

            var today = DateTime.Today;
            var yesterday = DateTime.Today.AddDays(-1);

            foreach (var order in completedOrders.OrderByDescending(o => o.CreatedAt))
            {
                var orderDate = order.CreatedAt.Date;
                System.Diagnostics.Debug.WriteLine($"Order {order.OrderNumber} CreatedAt: {order.CreatedAt}, Date: {orderDate}");

                if (orderDate == today)
                {
                    TodayOrders.Add(order);
                }
                else if (orderDate == yesterday)
                {
                    YesterdayOrders.Add(order);
                }
            }

        }

        private void RecallOrder(Order order)
        {
            if (order == null) return;

            // Example: navigate to order detail or add back to kitchen
            Application.Current.MainPage.DisplayAlert("Recall", $"Order {order.OrderNumber} recalled!", "OK");

            // You can also implement logic to move the order back to Paid/Preparing



        }
    }
}