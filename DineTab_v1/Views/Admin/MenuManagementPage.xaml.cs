using DineTab_v1.ViewModels.Admin;

namespace DineTab_v1.Views.Admin
{
    public partial class MenuManagementPage : ContentView
    {
        public MenuManagementPage()
        {
            InitializeComponent();
            BindingContext = new MenuManagementViewModel();

            Task.Run(async () => await ((MenuManagementViewModel)BindingContext).LoadMenuItemsAsync());
        }
    }
}
