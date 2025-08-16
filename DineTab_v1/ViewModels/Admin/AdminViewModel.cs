using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;

namespace DineTab_v1.ViewModels.Admin;

public class AdminViewModel : BaseViewModel
{
    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowMenuManagementCommand { get; }
    public ICommand ShowStaffManagementCommand { get; }
    public ICommand ShowReportsCommand { get; }

    public AdminViewModel(User user)
    {
        ShowDashboardCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "Dashboard"));
        ShowMenuManagementCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "MenuManagement"));
        ShowStaffManagementCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "StaffManagement"));
        ShowReportsCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "Reports"));
    }
}
