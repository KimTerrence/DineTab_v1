using DineTab_v1.ViewModels.Auth;

namespace DineTab_v1.Views.Auth;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage()
	{
		InitializeComponent();
		BindingContext = new ForgotPasswordViewModel();
	}
}