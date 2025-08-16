using DineTab_v1.ViewModels.Auth;

namespace DineTab_v1.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

        var vm = new LoginViewModel();
        vm.Navigation = Navigation;
        BindingContext = vm;
    }
}