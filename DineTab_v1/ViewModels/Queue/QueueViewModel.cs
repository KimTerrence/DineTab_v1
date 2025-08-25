using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Queue
{
    public class QueueViewModel : BindableObject
    {
        private readonly DatabaseService _dbService = new();
        private bool timerStarted = false;

        public ObservableCollection<Order> PreparingOrders { get; set; } = new();
        public ObservableCollection<Order> ReadyOrders { get; set; } = new();

        public ICommand LoadDataCommand { get; }

        public QueueViewModel()
        {
            LoadDataCommand = new Command(async () => await LoadData());
        }

        private async Task LoadData()
        {
            PreparingOrders.Clear();
            ReadyOrders.Clear();

            var preparing = await _dbService.GetPreparingOrdersAsync();
            foreach (var order in preparing)
            {
                // Set initial RemainingTime
                if (order.TargetTime.HasValue)
                {
                    var remaining = order.TargetTime.Value - DateTime.Now;
                    order.RemainingTime = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
                }

                PreparingOrders.Add(order);
            }

            var ready = await _dbService.GetReadyOrdersAsync();
            foreach (var order in ready)
            {
                ReadyOrders.Add(order);
            }

            // Start the countdown timer once
            if (!timerStarted)
            {
                StartCountdownTimer();
                timerStarted = true;
            }
        }

        private void StartCountdownTimer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                foreach (var order in PreparingOrders)
                {
                    if (order.TargetTime.HasValue)
                    {
                        var remaining = order.TargetTime.Value - DateTime.Now;
                        order.RemainingTime = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
                    }
                }
                return true; // repeat every second
            });
        }
    }
}
