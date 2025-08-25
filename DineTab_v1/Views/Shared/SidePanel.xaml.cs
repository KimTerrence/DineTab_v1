using DineTab_v1.Models;
using DineTab_v1.Views.Admin;
using Microsoft.Maui.Controls;
using System;

namespace DineTab_v1.Views.Shared;

public partial class SidePanel : ContentView
{
    private readonly User _currentUser;
    private readonly AdminPage _adminPage;

    // Track the currently active button
    private Button _activeButton;

    public SidePanel(User currentUser, AdminPage adminPage)
    {
        InitializeComponent();

        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _adminPage = adminPage ?? throw new ArgumentNullException(nameof(adminPage));

        // Attach click handlers
        DashboardButton.Clicked += (s, e) =>
        {
            _adminPage.ShowPage("Dashboard");
            SetActiveButton(DashboardButton);
        };

        MenuManagementButton.Clicked += (s, e) =>
        {
            _adminPage.ShowPage("MenuManagement");
            SetActiveButton(MenuManagementButton);
        };

        StaffManagementButton.Clicked += (s, e) =>
        {
            _adminPage.ShowPage("StaffManagement");
            SetActiveButton(StaffManagementButton);
        };

        ReportsButton.Clicked += (s, e) =>
        {
            _adminPage.ShowPage("Reports");
            SetActiveButton(ReportsButton);
        };

        NotificationButton.Clicked += (s, e) =>
        {
            _adminPage.ShowPage("Notification");
            SetActiveButton(NotificationButton);
        };

        SignOutButton.Clicked += async (s, e) =>
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert(
                "Sign Out", "Are you sure you want to sign out?", "Yes", "No");

            if (confirmed)
                Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
        };
    }

    private void SetActiveButton(Button button)
    {
        // Reset previous active button
        if (_activeButton != null)
            _activeButton.BackgroundColor = Color.FromArgb("#00000000");

        // Set new active button
        button.BackgroundColor = Color.FromArgb("#FA7E0E"); // active color
        _activeButton = button;
    }
}
