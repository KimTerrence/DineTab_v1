using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.KitchenStaff;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.KitchenStaff
{
    public class KitchenStaffViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Order> PendingOrders { get; set; } = new();
        public ObservableCollection<Order> PaidOrders { get; set; } = new();
        public ObservableCollection<Order> PreparingOrders { get; set; } = new();
        public ObservableCollection<Order> ReadyOrders { get; set; } = new();

        public ICommand CookCommand { get; }
        public ICommand PrepCommand { get; }
        public ICommand AddFiveMinutesCommand { get; }
        public ICommand MarkReadyCommand { get; }
        public ICommand MarkCompleteCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand SignOutCommand { get; }

        public KitchenStaffViewModel()
        {
            _databaseService = new DatabaseService();
            CookCommand = new Command<Order>(order => StartPreparing(order, "Cook"));
            PrepCommand = new Command<Order>(order => StartPreparing(order, "Prep"));
            AddFiveMinutesCommand = new Command<Order>(order => AddFiveMinutes(order));
            MarkReadyCommand = new Command<Order>(order => MarkReady(order));
            MarkCompleteCommand = new Command<Order>(order => MarkComplete(order));
            ShowHistoryCommand = new Command(LoadCompletedOrders);
            SignOutCommand = new Command(async () =>
            {
                bool confirmed = await Application.Current.MainPage.DisplayAlert(
                    "Sign Out", "Are you sure you want to sign out?", "Yes", "No");

                if (confirmed)
                    Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
            });

            LoadData();
            StartGlobalTimer();
        }

        private async void LoadData()
        {
            var orders = await _databaseService.GetAllOrdersAsync();

            PreparingOrders.Clear();
            PendingOrders.Clear();
            PaidOrders.Clear();
            ReadyOrders.Clear();

            foreach (var order in orders)
            {
                switch (order.Status)
                {
                    case "Pending": PendingOrders.Add(order); break;
                    case "Paid": PaidOrders.Add(order); break;
                    case "Preparing":
                        // Only set default if null
                        if (!order.TargetTime.HasValue)
                        {
                            order.TargetTime = DateTime.Now.AddMinutes(20);
                           // await _databaseService.UpdateOrderPreparingAsync(order.OrderId, "Preparing", order.TargetTime.Value);
                        }

                        // Calculate RemainingTime based on TargetTime
                        order.RemainingTime = order.TargetTime.Value - DateTime.Now;
                        PreparingOrders.Add(order);
                        break;
                    case "Ready": ReadyOrders.Add(order); break;
                }
            }
        }



        private void StartGlobalTimer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                var now = DateTime.Now;

                foreach (var order in PreparingOrders)
                {
                    if (!order.TargetTime.HasValue) continue;

                    var remaining = order.TargetTime.Value - now;
                    order.RemainingTime = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
                }

                return true; // repeat every second
            });
        }

        private async void StartPreparing(Order order, string mode)
        {
            if (order == null) return;

            order.Status = "Preparing";

            // Set TargetTime based on mode
            if (mode == "Cook")
                order.TargetTime = DateTime.Now.AddMinutes(20);
            else if (mode == "Prep")
                order.TargetTime = DateTime.Now.AddMinutes(15);
            else
                order.TargetTime = DateTime.Now.AddMinutes(20); // default fallback

            order.RemainingTime = order.TargetTime.Value - DateTime.Now;

            await _databaseService.UpdateOrderPreparingAsync(order.OrderId, "Preparing", order.TargetTime);

            if (PaidOrders.Contains(order)) PaidOrders.Remove(order);
            if (!PreparingOrders.Contains(order)) PreparingOrders.Add(order);
        }



        private async void AddFiveMinutes(Order order)
        {
            if (order == null) return;

            if (!order.TargetTime.HasValue)
                order.TargetTime = DateTime.Now;

            order.TargetTime = order.TargetTime.Value.AddMinutes(5);
            order.RemainingTime = order.TargetTime.Value - DateTime.Now;

            await _databaseService.UpdateOrderPreparingAsync(order.OrderId, "Preparing", order.TargetTime.Value);
        }

        private async void MarkReady(Order order)
        {
            if (order == null) return;

            order.Status = "Ready";
            order.RemainingTime = TimeSpan.Zero;

            await _databaseService.UpdateOrderPreparingAsync(order.OrderId, "Ready", DateTime.Now);

            if (PreparingOrders.Contains(order)) PreparingOrders.Remove(order);
            if (!ReadyOrders.Contains(order)) ReadyOrders.Add(order);
        }

        private async void MarkComplete(Order order)
        {
            if (order == null) return;

            order.Status = "Complete";

            await _databaseService.UpdateOrderPreparingAsync(order.OrderId, "Complete", DateTime.Now);

            if (ReadyOrders.Contains(order)) ReadyOrders.Remove(order);
        }

        public async void LoadCompletedOrders()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HistoryPage());
        }
    }
}
