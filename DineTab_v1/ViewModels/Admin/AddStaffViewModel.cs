using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace DineTab_v1.ViewModels.Admin
{
    public class AddStaffViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }

        public ICommand AddAccountCommand { get; }
        public ICommand CancelCommand { get; }

        public INavigation Navigation { get; set; }

        public AddStaffViewModel()
        {
            CancelCommand = new Command(OnCancel);
            AddAccountCommand = new Command(OnAdd);
        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async void OnAdd()
        {
            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match", "OK"); //Display error message if passwords do not match
                return;
            }

            User newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                Role = Role
            };

            bool success = await _dbService.AddStaffAsync(newUser);

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Staff added successfully", "OK"); //Show Success message
                await Application.Current.MainPage.Navigation.PopModalAsync(); //Go back to the previous page
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add staff", "OK"); //Show Error message
            }
        }
    }
}
