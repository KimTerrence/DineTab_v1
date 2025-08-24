using DineTab_v1.ViewModels.Cashier;

namespace DineTab_v1.Views.Cashier;

public partial class CreateOrderPage : ContentPage
{
	public CreateOrderPage()
	{
		InitializeComponent();
        BindingContext = new CreateOrderViewModel();
    }
}