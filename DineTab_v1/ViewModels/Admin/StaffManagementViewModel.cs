using System.Windows.Input;
using DineTab_v1.Views.Admin;

namespace DineTab_v1.ViewModels.Admin
{
    public class StaffManagementViewModel : BaseViewModel
    {
        public ICommand AddAccountCommand { get; }
        public ICommand GoToRemoveStaffCommand { get; }
        public ICommand GoToModifyStaffCommand { get; }

        public StaffManagementViewModel()
        {
            AddAccountCommand = new Command(OnAddAccount);
            GoToRemoveStaffCommand = new Command(GoToRemoveStaff);
            GoToModifyStaffCommand = new Command(GoToModifyStaff);
        }

        private async void OnAddAccount()
        {
            // Navigate to modal AddStaffPage
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddStaffPage());
        }

        private async void GoToRemoveStaff()
        {
            // Navigate to modal AddStaffPage
            await Application.Current.MainPage.Navigation.PushModalAsync(new RemoveStaffPage());
        }

        private async void GoToModifyStaff()
        {
            // Navigate to modal AddStaffPage
            await Application.Current.MainPage.Navigation.PushModalAsync(new ModifyStaffPage());
        }
    }
}
