using System;
using System.Collections.Generic;

namespace DineTab_v1.Models
{
    public class Order
    {
        public int OrderId { get; set; } // Auto-increment PK in DB
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        
    }
}
