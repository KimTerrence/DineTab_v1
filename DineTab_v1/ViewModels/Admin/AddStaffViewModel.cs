using DineTab_v1.Models;
using DineTab_v1.Views.Admin;
using Microsoft.Data.SqlClient;
using DineTab_v1.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace DineTab_v1.ViewModels.Admin
{
    public class AddStaffViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        // Bindable properties for profile image
        private ImageSource profileImage = "icon.png";
        public ImageSource ProfileImage
        {
            get => profileImage;
            set => SetProperty(ref profileImage, value);
        }

        // Staff info properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }

        // Store filename for database
        public string ImageFileName { get; set; }

        // Commands
        public ICommand UploadPhotoCommand { get; }
        public ICommand AddAccountCommand { get; }
        public ICommand CancelCommand { get; }

        public INavigation Navigation { get; set; }
        private byte[] ImageBytes { get; set; }

        // Constructor
        public AddStaffViewModel()
        {
            UploadPhotoCommand = new Command(async () => await PickAndSaveImageAsync());
            CancelCommand = new Command(OnCancel);
            AddAccountCommand = new Command(OnAdd);
        }

        private async Task PickAndSaveImageAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a profile photo"
                });

                if (result == null) return;

                using (var input = await result.OpenReadAsync())
                using (var ms = new MemoryStream())
                {
                    await input.CopyToAsync(ms);
                    ImageBytes = ms.ToArray(); // store image in memory
                }

                // Show in UI immediately (preview)
                ProfileImage = ImageSource.FromStream(() => new MemoryStream(ImageBytes));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to load image: {ex.Message}", "OK");
            }
        }

        // Cancel command to close the modal
        private async void OnCancel()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync(); //close modal
            }
            catch { }
        }

        // Add command to save new staff member
        private async void OnAdd()
        {
            try
            {
                if (Password != ConfirmPassword)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match", "OK");
                    return;
                }

                if (Password.Length < 8 || !Regex.IsMatch(Password, @"[!@#$%^&*(),.?""':{}|<>]"))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Password must be at least 8 characters and contain at least one special character.", "OK");
                    return;
                }

                bool success = await _databaseService.AddStaffAsync(
                    FirstName, LastName, Email, Password, Role, ImageBytes);

                if (success)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    await Application.Current.MainPage.Navigation.PushModalAsync(new SuccessPopUp());
                    MessagingCenter.Send(this, "StaffUpdated");
                }
            }
            catch (Exception ex)
            {
               // await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }       
    }
}
