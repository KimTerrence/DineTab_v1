using DineTab_v1.Models;
using DineTab_v1.Services;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DineTab_v1.ViewModels.Admin
{
    public class ModifyStaffViewModel : INotifyPropertyChanged
    {

        private int Id;
        public int UserId { get => Id; set => SetProperty(ref Id, value); }

        private string firstName;
        private string lastName;
        private string email;
        private string role;

        public string Role
        {
            get => role;
            set
            {
                if (role != value)
                {
                    role = value;
                    OnPropertyChanged();
                    // Update checkboxes
                    OnPropertyChanged(nameof(IsAdmin));
                    OnPropertyChanged(nameof(IsCashier));
                    OnPropertyChanged(nameof(IsKitchenStaff));
                }
            }
        }

        private bool isActive;
        public string StatusText => IsActive ? "Active" : "Inactive";        
        public string FullName { get; }

        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }
        public string Email { get => email; set => SetProperty(ref email, value); }

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (SetProperty(ref isActive, value))
                {
                    isActive = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StatusText)); // update label when switch changes
                }
            }
        }

        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set => SetProperty(ref newPassword, value);
        }

        private bool isResetPasswordVisible;
        public bool IsResetPasswordVisible
        {
            get => isResetPasswordVisible;
            set => SetProperty(ref isResetPasswordVisible, value);
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        // Bindable property for image
        private ImageSource profileImage;
        public ImageSource ProfileImage
        {
            get => profileImage;
            set => SetProperty(ref profileImage, value);
        }

        // Store filename for DB
        private string imageFileName;
        public string ImageFileName
        {
            get => imageFileName;
            set => SetProperty(ref imageFileName, value);
        }

        // Command to pick a new image
        public ICommand UploadPhotoCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ToggleResetPasswordCommand { get; }

        private readonly DatabaseService _databaseService;
        public ModifyStaffViewModel(User user)
        {

            _databaseService = new DatabaseService(); // call  database service

            Id = user.Id;
            FullName = user.FullName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Role = user.Role;
            IsActive = user.Status == "Active";

            ProfileImage = !string.IsNullOrEmpty(user.ProfileImageFile)
       ? ImageSource.FromFile(Path.Combine(FileSystem.AppDataDirectory, user.ProfileImageFile))
       : "icon.png";

            ImageFileName = user.ProfileImageFile;

            // Commands
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            ToggleResetPasswordCommand = new Command(() => IsResetPasswordVisible = !IsResetPasswordVisible);
            UploadPhotoCommand = new Command(async () => await PickAndSaveImageAsync());
        }


        // Pick and save new profile image
        private async Task PickAndSaveImageAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a profile photo"
                });

                if (result != null)
                {
                    var stream = await result.OpenReadAsync();

                    // Save to AppDataDirectory
                    ImageFileName = result.FileName;
                    var savePath = Path.Combine(FileSystem.AppDataDirectory, ImageFileName);
                    using var fileStream = File.OpenWrite(savePath);
                    await stream.CopyToAsync(fileStream);

                    // Update ImageSource
                    ProfileImage = ImageSource.FromFile(savePath);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to select image: {ex.Message}", "OK");
            }
        }


        private async void OnSave()
        {
            // Save user info
            try
            {
                // Validate password if reset section is visible
                if (IsResetPasswordVisible)
                {
                    if (string.IsNullOrWhiteSpace(NewPassword))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Please enter a new password.", "OK");
                        return;
                    }

                    // Check password length and special character
                    if (NewPassword.Length < 8 ||
                        !System.Text.RegularExpressions.Regex.IsMatch(NewPassword, @"[!@#$%^&*(),.?""':{}|<>]"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error",
                            "Password must be at least 8 characters and contain at least one special character.", "OK");
                        return;
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                        return;
                    }
                }

                // Create updated user object
                var updatedUser = new User
                {
                    Id = this.Id,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Email = this.Email,
                    Role = this.Role,
                    Status = this.IsActive ? "Active" : "Inactive", // convert bool to string
                    Password = IsResetPasswordVisible ? NewPassword : null, // only update if reset
                    ProfileImageFile = this.ImageFileName // store only filename in DB
                };
                
                // Call your service to update user in database
                bool success = await _databaseService.UpdateStaffAsync(updatedUser);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "User updated successfully!", "OK");
                    try
                    {
                        await Application.Current.MainPage.Navigation.PopModalAsync(); //close modal
                    }
                    catch (Exception ex) { }
                    MessagingCenter.Send(this, "StaffUpdated");

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to update user.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnCancel()
        {
            Application.Current.MainPage.Navigation.PopModalAsync();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        // Convenience bools for binding CheckBoxes
        public bool IsAdmin
        {
            get => Role == "Admin";
            set { if (value) Role = "Admin"; }
        }
        public bool IsCashier
        {
            get => Role == "Cashier";
            set { if (value) Role = "Cashier"; }
        }
        public bool IsKitchenStaff
        {
            get => Role == "Kitchen Staff";
            set { if (value) Role = "Kitchen Staff"; }
        }
    }
}
