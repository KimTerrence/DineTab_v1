using DineTab_v1.ViewModels.Auth;
using DineTab_v1.Views.Customer;

namespace DineTab_v1.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false); // hides the header
        // Pass this page's Navigation to the ViewModel
        BindingContext = new LoginViewModel(this.Navigation);
    }
}