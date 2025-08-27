using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DineTab_v1.Models
{
    //Order Model 
    public class Order : INotifyPropertyChanged
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public ObservableCollection<OrderItem> Items { get; set; } = new();
        public string Status { get; set; }

        //   Target time for countdown persistence
        public DateTime? TargetTime { get; set; }

        //  PreparingUntil for live countdown
        public DateTime? PreparingUntil { get; set; }

        //  Writable RemainingTime
        private TimeSpan remainingTime;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            set
            {
                if (remainingTime != value)
                {
                    remainingTime = value;
                    OnPropertyChanged(nameof(RemainingTime));
                    OnPropertyChanged(nameof(RemainingTimeString));
                }
            }
        }

        public string RemainingTimeString =>
            RemainingTime > TimeSpan.Zero
                ? $"{RemainingTime.Minutes:D2}:{RemainingTime.Seconds:D2}"
                : "00:00";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
