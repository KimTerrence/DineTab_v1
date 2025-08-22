using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineTab_v1.Models
{
    public class OrderDisplay
    {
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public ObservableCollection<OrderItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.TotalPrice);
    }

}
