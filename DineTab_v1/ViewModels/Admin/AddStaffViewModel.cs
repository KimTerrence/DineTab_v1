using DineTab_v1.Models;
using DineTab_v1.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.Admin
{
    public class AddStaffViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService = new DatabaseService();

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

        //To Add Staff
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

                // Ensure folder exists
                var imagesFolder = AppPaths.GetImagesFolder();
                Directory.CreateDirectory(imagesFolder);

                // Unique filename
                ImageFileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
                var savePath = Path.Combine(imagesFolder, ImageFileName);

                using (var input = await result.OpenReadAsync())
                using (var output = File.Create(savePath))
                {
                    await input.CopyToAsync(output);
                    await output.FlushAsync();
                }

                // Show in UI immediately
                ProfileImage = ImageSource.FromFile(savePath);

                await Application.Current.MainPage.DisplayAlert("Saved", savePath, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to save image: {ex.Message}", "OK");
            }
        }



        // Cancel command to close the modal
        private async void OnCancel()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync(); //close modal
            }
            catch (Exception ex) { }

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

                User newUser = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Role = Role,
                    ProfileImageFile = ImageFileName // store only filename in DB
                };

                bool success = await _dbService.AddStaffAsync(newUser);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Staff added successfully", "OK");
                    try
                    {
                        await Application.Current.MainPage.Navigation.PopModalAsync(); //close modal
                    }
                    catch (Exception ex) { }
                    MessagingCenter.Send(this, "StaffUpdated");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to add staff", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill all fields", "OK");
            }
        }
    }
}
