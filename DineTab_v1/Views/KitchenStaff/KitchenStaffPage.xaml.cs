using DineTab_v1.ViewModels.KitchenStaff;	

namespace DineTab_v1.Views.KitchenStaff;

public partial class KitchenStaffPage : ContentPage
{
	public KitchenStaffPage()
	{
		InitializeComponent();
		BindingContext = new KitchenStaffViewModel();
        NavigationPage.SetHasNavigationBar(this, false);

    }
}