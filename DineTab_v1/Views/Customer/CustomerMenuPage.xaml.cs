using DineTab_v1.ViewModels.Customer;

namespace DineTab_v1.Views.Customer;

public partial class CustomerMenuPage : ContentPage
{
    public CustomerMenuPage(string orderType)
    {
        InitializeComponent();

        // Pass orderType to the ViewModel
        BindingContext = new CustomerMenuViewModel(orderType);
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
