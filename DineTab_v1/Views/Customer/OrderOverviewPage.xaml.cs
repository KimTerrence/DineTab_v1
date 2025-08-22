using DineTab_v1.ViewModels.Customer;
using System.Collections.ObjectModel;
using DineTab_v1.Models;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Customer
{
    public partial class OrderOverviewPage : ContentPage
    {
        public OrderOverviewPage(OrderOverviewViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
