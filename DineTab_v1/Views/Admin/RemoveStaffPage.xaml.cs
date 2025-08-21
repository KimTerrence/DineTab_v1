using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Models;

using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin;

public partial class RemoveStaffPage : ContentPage
{
    [Preserve(AllMembers = true)]
    public RemoveStaffPage(User user)
	{
		InitializeComponent();
        // Bind to ViewModel or directly set fields
        BindingContext = new RemoveStaffViewModel(user);
    }
}