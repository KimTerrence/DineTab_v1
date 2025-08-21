using DineTab_v1.ViewModels.Customer;

namespace DineTab_v1.Views.Customer;

public partial class CustomerMenuPage : ContentPage
{
	public CustomerMenuPage()
	{
		InitializeComponent();
        BindingContext = new CustomerMenuViewModel();

        NavigationPage.SetHasNavigationBar(this, false);
    }
}