using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.Admin
{
    public class StaffManagementViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        public ObservableCollection<User> StaffList { get; set; } = new ObservableCollection<User>();

        public ICommand AddAccountCommand { get; }
        public ICommand GoToRemoveStaffCommand { get; }
        public ICommand GoToModifyStaffCommand { get; }

        public StaffManagementViewModel()
        {
            _dbService = new DatabaseService();

            // Load staff/User from SQL Server
            LoadUser();

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

        private async void LoadUser()
        {
            var staffFromDb = await _dbService.GetAllStaffAsync();

            StaffList.Clear();
            foreach (var s in staffFromDb)
                StaffList.Add(s);
        }

    }
}
