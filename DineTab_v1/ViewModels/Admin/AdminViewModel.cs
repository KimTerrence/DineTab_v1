using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;

namespace DineTab_v1.ViewModels.Admin;

public class AdminViewModel : BaseViewModel
{
    private readonly User _currentUser;

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowMenuManagementCommand { get; }
    public ICommand ShowStaffManagementCommand { get; }
    public ICommand ShowReportsCommand { get; }
    public ICommand ShowNotificationCommand { get; }
    public ICommand ShowPOSCommand { get; }
    public ICommand ShowKitchenCommand { get; }
    public ICommand SignOutCommand { get; }

    public AdminViewModel(User user)
    {
        _currentUser = user ?? throw new ArgumentNullException(nameof(user));

        ShowDashboardCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "Dashboard"));
        ShowMenuManagementCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "MenuManagement"));
        ShowStaffManagementCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "StaffManagement"));
        ShowReportsCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "Reports"));
        ShowNotificationCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "Notification"));
        ShowPOSCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "POS"));
        ShowKitchenCommand = new Command(() => MessagingCenter.Send(this, "MenuSelected", "Kitchen"));

        SignOutCommand = new Command(async () =>
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert(
                "Sign Out", "Are you sure you want to sign out?", "Yes", "No");

            if (confirmed)
                Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
        });
    }
}
