using System.Collections.ObjectModel;
using DineTab_v1.Models;
using DineTab_v1.ViewModels.Cashier;

namespace DineTab_v1.Views.Cashier;
    public partial class OverviewPage : ContentPage
    {
        public OverviewPage(string orderId, ObservableCollection<OrderItem> items, string total)
        {
            InitializeComponent();
            BindingContext = new OverviewViewModel(orderId, items, total);
        }
    }
