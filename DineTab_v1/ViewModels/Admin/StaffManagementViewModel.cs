using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

            // Subscribe to updates from Add/Modify/Delete staff
            MessagingCenter.Subscribe<AddStaffViewModel>(this, "StaffUpdated", (sender) => LoadUser());
            MessagingCenter.Subscribe<ModifyStaffViewModel>(this, "StaffUpdated", (sender) => LoadUser());
            MessagingCenter.Subscribe<RemoveStaffViewModel>(this, "StaffUpdated", (sender) => LoadUser());

            // Commands
            AddAccountCommand = new Command(OnAddAccount);
            GoToRemoveStaffCommand = new Command<User>(async (user) => await GoToRemoveStaff(user));
            GoToModifyStaffCommand = new Command<User>(async (user) => await GoToModifyStaff(user));

            // Load staff from DB
            LoadUser();
        }

        private async void OnAddAccount()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddStaffPage());
        }

        private async Task GoToRemoveStaff(User user)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new RemoveStaffPage(user));
        }

        private async Task GoToModifyStaff(User user)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ModifyStaffPage(user));
        }

        private async void LoadUser()
        {
            try
            {
                var staffFromDb = await _dbService.GetAllStaffAsync();
                StaffList.Clear();

                foreach (var u in staffFromDb)
                {
                    StaffList.Add(u);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load staff: {ex.Message}", "OK");
            }
        }
    }
}
