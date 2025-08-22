
using DineTab_v1.Models;
using DineTab_v1.Views.Admin;
using Microsoft.Maui.Controls;
using System;

namespace DineTab_v1.Views.Shared;

public partial class SidePanel : ContentView
{
    private readonly User _currentUser;
    private readonly AdminPage _adminPage;

    public SidePanel(User currentUser, AdminPage adminPage)
    {
        InitializeComponent();

        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _adminPage = adminPage ?? throw new ArgumentNullException(nameof(adminPage));

        // Attach click handlers
        DashboardButton.Clicked += (s, e) => _adminPage.ShowPage("Dashboard");
        MenuManagementButton.Clicked += (s, e) => _adminPage.ShowPage("MenuManagement");
        StaffManagementButton.Clicked += (s, e) => _adminPage.ShowPage("StaffManagement");
        ReportsButton.Clicked += (s, e) => _adminPage.ShowPage("Reports");
        NotificationButton.Clicked += (s, e) => _adminPage.ShowPage("Notification");

        SignOutButton.Clicked += async (s, e) =>
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert(
                "Sign Out", "Are you sure you want to sign out?", "Yes", "No");

            if (confirmed)
                Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
        };
    }
}
