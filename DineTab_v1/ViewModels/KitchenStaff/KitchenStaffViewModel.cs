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

        public KitchenStaffViewModel()
        {
            _databaseService = new DatabaseService();
            CookCommand = new Command<Order>(order => StartPreparing(order, "Cook"));
            PrepCommand = new Command<Order>(order => StartPreparing(order, "Prep"));
            AddFiveMinutesCommand = new Command<Order>(order => AddFiveMinutes(order));
            MarkReadyCommand = new Command<Order>(order => MarkReady(order));
            MarkCompleteCommand = new Command<Order>(order => MarkComplete(order));
            ShowHistoryCommand = new Command(LoadCompletedOrders);

            LoadData();
            StartGlobalTimer();
        }

        private async void LoadData()
        {
            var orders = await _databaseService.GetAllOrdersAsync();

            foreach (var order in orders)
            {
                if (order.Status == "Preparing" && order.TargetTime.HasValue)
                {
                    order.RemainingTime = order.TargetTime.Value - DateTime.Now;
              
                }

                switch (order.Status)
                {
                    case "Pending": PendingOrders.Add(order); break;
                    case "Paid": PaidOrders.Add(order); break;
                    case "Preparing": PreparingOrders.Add(order); break;
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
                    if (!order.PreparingUntil.HasValue) continue;

                    order.RemainingTime = order.PreparingUntil.Value - now;

                    // ✅ Do NOT auto change to Ready when 00:00
                    if (order.RemainingTime <= TimeSpan.Zero)
                    {
                        order.RemainingTime = TimeSpan.Zero;
                        // Status stays "Preparing" so user can still add +5 min
                    }
                }

                return true;
            });
        }

        private async void StartPreparing(Order order, string mode)
        {
            if (order == null) return;

            order.Status = "Preparing";
            order.TargetTime = DateTime.Now.AddMinutes(20); // save to DB
            order.PreparingUntil = order.TargetTime;

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


        private async void MarkReady(Order order) // ✅ explicit ready action
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

            // Update MS SQL
            await _databaseService.UpdateOrderPreparingAsync(order.OrderId, "Complete", DateTime.Now);

            // Remove from ReadyOrders
            if (ReadyOrders.Contains(order)) ReadyOrders.Remove(order);
        }
        public async void LoadCompletedOrders()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HistoryPage());

        }

    }
}
