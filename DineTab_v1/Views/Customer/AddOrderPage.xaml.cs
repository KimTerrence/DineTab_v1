using System.Collections.ObjectModel;
using DineTab_v1.Models;
using DineTab_v1.ViewModels.Customer;

namespace DineTab_v1.Views.Customer;

public partial class AddOrderPage : ContentPage
{
    public AddOrderPage(Item item, ObservableCollection<OrderItem> orderItems)
    {
        InitializeComponent();

        BindingContext = new AddOrderViewModel(orderItems)
        {
            SelectedItem = item
        };
    }
}
