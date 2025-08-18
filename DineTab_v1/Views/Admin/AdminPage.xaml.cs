using DineTab_v1.Models;
using DineTab_v1.ViewModels.Admin;

namespace DineTab_v1.Views.Admin
{
    public partial class AdminPage : ContentPage
    {
        public AdminPage(User currentUser)
        {
            InitializeComponent();

            var viewModel = new AdminViewModel(currentUser);
            BindingContext = viewModel;

            // Load default Dashboard
            MainPanelContainer.Content = new Dashboard();

            // Listen for side panel menu selections
            MessagingCenter.Subscribe<AdminViewModel, string>(this, "MenuSelected", (sender, page) =>
            {
                MainPanelContainer.Content = page switch
                {
                    "Dashboard" => new Dashboard(),
                    "MenuManagement" => new MenuManagementPage(),
                    "StaffManagement" => new StaffManagementPage(),
                    "Reports" => new ReportsPage(),
                    _ => new Dashboard()
                };
            });

            // Listen for Modify Categories from MenuManagement
            MessagingCenter.Subscribe<MenuManagementViewModel>(this, "ShowModifyCategories", (sender) =>
            {
                 Navigation.PushAsync(new ModifyCategoriesPage());
            });

            MessagingCenter.Subscribe<MenuManagementViewModel>(this, "ShowModifyCategories", (sender) =>
            {
                 Navigation.PushAsync(new ModifyCategoriesPage());
            });

            MessagingCenter.Subscribe<ModifyCategoriesViewModel>(this, "BackToMenuManagement", (sender) =>
            {
                MainPanelContainer.Content = new MenuManagementPage();
            });


        }
    }
}