using DineTab_v1.Models;
using System;
using System.Collections.ObjectModel;

namespace DineTab_v1.Views.Cashier;

public partial class ReceiptPage : ContentPage
{
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public ObservableCollection<OrderItem> OrderItems { get; set; }
    public decimal TotalAmount { get; set; }

    public ReceiptPage(string orderNumber, ObservableCollection<OrderItem> items, decimal totalAmount)
    {
        InitializeComponent();
        OrderNumber = orderNumber;
        OrderDate = DateTime.Now;
        OrderItems = items;
        TotalAmount = totalAmount;

        BindingContext = this;
    }
}
