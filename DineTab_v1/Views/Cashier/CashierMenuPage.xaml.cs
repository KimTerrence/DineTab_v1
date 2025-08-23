using DineTab_v1.ViewModels.Cashier;


namespace DineTab_v1.Views.Cashier;

public partial class CashierMenuPage : ContentPage
{
    public CashierMenuPage()
    {
        InitializeComponent();
        BindingContext = new CashierMenuViewModel();
    }
}
