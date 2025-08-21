using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Models;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin;

public partial class ModifyStaffPage : ContentPage
{
    [Preserve(AllMembers = true)]
    public User CurrentUser { get; set; }

    public ModifyStaffPage(User user)
    {
        InitializeComponent();
        CurrentUser = user;

        // Bind to ViewModel or directly set fields
        BindingContext = new ModifyStaffViewModel(user);
    }
}
