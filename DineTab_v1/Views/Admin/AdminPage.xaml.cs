using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.Views.Shared;

namespace DineTab_v1.Views.Admin;

public partial class AdminPage : ContentPage
{
    private readonly User _currentUser;

    public AdminPage(User currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

        NavigationPage.SetHasNavigationBar(this, false);

        // Create side panel in code-behind and pass current page reference
        SidePanelContainer.Content = new SidePanel(_currentUser, this);

        // Load default dashboard
        MainPanelContainer.Content = new Dashboard();
    }

    // Method to switch main panel content
    public void ShowPage(string page)
    {
        MainPanelContainer.Content = page switch
        {
            "Dashboard" => new Dashboard(),
            "MenuManagement" => new MenuManagementPage(),
            "StaffManagement" => new StaffManagementPage(),
            "Reports" => new ReportsPage(),
            "Notification" => new NotificationPage(),
            _ => new Dashboard()
        };
    }
}
