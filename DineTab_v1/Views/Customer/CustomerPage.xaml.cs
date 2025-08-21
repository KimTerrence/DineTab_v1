namespace DineTab_v1.Views.Customer;
using DineTab_v1.ViewModels.Customer;

public partial class CustomerPage : ContentPage
{
	public CustomerPage()
	{
		InitializeComponent();
		BindingContext = new CustomerViewModel();
		// Hide the navigation bar for this page
		NavigationPage.SetHasNavigationBar(this, false);

    }
}