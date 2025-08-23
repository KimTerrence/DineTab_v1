
using DineTab_v1.Models;
using DineTab_v1.ViewModels.Cashier;
using System.Collections.ObjectModel;

namespace DineTab_v1.Views.Cashier;

public partial class PaymentPage : ContentPage
{
    public PaymentPage(string orderNumber, decimal totalAmount, ObservableCollection<OrderItem> items)
    {
        InitializeComponent();
        BindingContext = new PaymentViewModel(orderNumber, totalAmount, items);
    }
}
