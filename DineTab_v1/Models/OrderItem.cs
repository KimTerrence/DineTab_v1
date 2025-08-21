using Microsoft.Maui.Controls;

namespace DineTab_v1.Models
{
    public class OrderItem : BindableObject
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }

        private int quantity = 1;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice)); // 🔑 Updates UI
                }
            }
        }

        public decimal TotalPrice => Price * Quantity;
    }
}
