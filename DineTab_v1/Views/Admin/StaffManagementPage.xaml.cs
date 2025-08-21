using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin;

public partial class StaffManagementPage : ContentView
{
    [Preserve(AllMembers = true)]
    public StaffManagementPage()
	{
		InitializeComponent();
		BindingContext = new StaffManagementViewModel();
    }
}