using System.Linq;
using Microsoft.Maui.Controls;

namespace DineTab_v1.Models
{
    public class OrderItem : BindableObject
    {
        private int quantity;

        public string Name { get; set; }
        public decimal UnitPrice { get; set; }

        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
