using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin
{
    [Preserve(AllMembers = true)]
    public partial class MenuManagementPage : ContentView
    {

        public MenuManagementPage()
        {
            InitializeComponent();
            BindingContext = new MenuManagementViewModel();

            _ = Dispatcher.DispatchAsync(async () =>
            {
                await ((MenuManagementViewModel)BindingContext).LoadMenuItemsAsync();
            });
            Task.Run(async () => await ((MenuManagementViewModel)BindingContext).LoadMenuItemsAsync());
        }
    }
}
