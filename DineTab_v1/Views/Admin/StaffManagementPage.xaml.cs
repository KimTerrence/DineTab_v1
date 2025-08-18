using DineTab_v1.ViewModels.Admin;

namespace DineTab_v1.Views.Admin;

public partial class StaffManagementPage : ContentView
{
	public StaffManagementPage()
	{
		InitializeComponent();
		BindingContext = new StaffManagementViewModel();
    }
}