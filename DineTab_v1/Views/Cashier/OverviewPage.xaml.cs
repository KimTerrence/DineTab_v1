using DineTab_v1.Models;
using DineTab_v1.ViewModels.Cashier;
using System.Collections.ObjectModel;

namespace DineTab_v1.Views.Cashier
{
    public partial class OverviewPage : ContentPage
    {
        public OverviewPage(string orderNumber, ObservableCollection<OrderItem> items, string total, string moneyReceived, string change)
        {
            InitializeComponent();
            BindingContext = new OverviewViewModel(orderNumber, items, total, moneyReceived, change);
        }
    }
}
