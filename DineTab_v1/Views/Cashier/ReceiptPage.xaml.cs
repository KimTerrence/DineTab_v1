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

    public decimal CashReceived { get; set; }
    public decimal Change { get; set; }
    public string PaymentMethod { get; set; }

    public ReceiptPage(string orderNumber, ObservableCollection<OrderItem> items, decimal totalAmount, decimal cashReceived, string paymentMethod)
    {
        InitializeComponent();

        OrderNumber = orderNumber;
        OrderDate = DateTime.Now;
        OrderItems = items;
        TotalAmount = totalAmount;
        CashReceived = cashReceived;
        PaymentMethod = paymentMethod;
        Change = cashReceived - totalAmount;

        OrderNumber = orderNumber;
        OrderDate = DateTime.Now;
        OrderItems = items;
        TotalAmount = totalAmount;

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Wait for 3 seconds
        await Task.Delay(3000);

        // Navigate to Home page (replace with your actual HomePage class)
        Application.Current.MainPage = new NavigationPage(new CashierMenuPage());
    }
}
