using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Admin
{
    public class RemoveStaffViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public User SelectedStaff { get; }

        // Bindable properties
        public string FullName => $"{SelectedStaff.FirstName} {SelectedStaff.LastName}";
        public string Role => SelectedStaff.Role;
        public int EmployeeId => SelectedStaff.Id;
       // public string LastLogin => SelectedStaff.LastLogin?.ToString("g") ?? "N/A";

        public ICommand CancelCommand { get; }
        public ICommand RemoveStaffCommand { get; }

        public RemoveStaffViewModel(User user)
        {
            SelectedStaff = user;
            _dbService = new DatabaseService();

            CancelCommand = new Command(OnCancel);
            RemoveStaffCommand = new Command(async () => await OnRemoveStaff());
        }

        private async void OnCancel()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnCancel: {ex.Message}");
            }          
        }

        private async Task OnRemoveStaff()
        {
            try
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
               "Confirm Removal",
               $"Are you sure you want to remove {FullName}?",
               "Yes", "No");

                if (!confirm) return;

                bool success = await _dbService.DeleteStaffAsync(SelectedStaff.Id);

                if (success)
                {
                    try
                    {
                        await Application.Current.MainPage.Navigation.PopModalAsync(); //close modal
                    }
                    catch (Exception ex) { }
                    MessagingCenter.Send(this, "StaffUpdated");

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to remove staff.", "OK");
                }
            }
            catch (Exception ex) { 
            Console.WriteLine(ex.Message);
            }
           
        }
    }
}
