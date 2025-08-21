using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Views.Admin;

namespace DineTab_v1.Views.Admin;

public partial class AdminPage : ContentPage
{
    public AdminPage(User currentUser)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        // Bind ViewModel
        BindingContext = new AdminViewModel(currentUser ?? throw new ArgumentNullException(nameof(currentUser)));

        // Load default dashboard
        MainPanelContainer.Content = new Dashboard();

        // Subscribe to side menu selection
        MessagingCenter.Subscribe<AdminViewModel, string>(this, "MenuSelected", OnMenuSelected);
    }

    private void OnMenuSelected(AdminViewModel sender, string page)
    {
        MainPanelContainer.Content = page switch
        {
            "Dashboard" => new Dashboard(),
            "MenuManagement" => new MenuManagementPage(),
            "StaffManagement" => new StaffManagementPage(),
            "Reports" => new ReportsPage(),
            "Notification" => new NotificationPage(),
            //"POS" => new CashierMenuView(),
            //"Kitchen" => new KitchenDisplayView(),
            _ => new Dashboard()
        };
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<AdminViewModel, string>(this, "MenuSelected");
    }
}
