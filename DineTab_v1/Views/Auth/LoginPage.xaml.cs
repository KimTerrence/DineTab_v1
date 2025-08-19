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
    private void LoginButton_Clicked(object sender, EventArgs e)
    {
        Overlay.IsVisible = true;
        LoginFormFrame.IsVisible = true;
    }

    private void CloseLoginForm_Clicked(object sender, EventArgs e)
    {
        Overlay.IsVisible = false;
        LoginFormFrame.IsVisible = false;
    }
    private void OrderButton_Clicked(object sender, EventArgs e)
    {
        // Your order button logic
        DisplayAlert("Order", "You clicked to order!", "OK");
    }
}