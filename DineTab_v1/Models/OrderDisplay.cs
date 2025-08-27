using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineTab_v1.Models
{
    //Model for displaying order with total calculation
    public class OrderDisplay
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public ObservableCollection<OrderItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.TotalPrice);
    }

}
