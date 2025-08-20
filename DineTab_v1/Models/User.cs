using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.ComponentModel;
using System.IO;

namespace DineTab_v1.Models
{
    public class User : INotifyPropertyChanged
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }

        private string profileImageFile;
        public string ProfileImageFile
        {
            get => profileImageFile;
            set
            {
                if (profileImageFile != value)
                {
                    profileImageFile = value;
                    OnPropertyChanged(nameof(ProfileImageFile));
                    OnPropertyChanged(nameof(ProfileImageSource));
                }
            }
        }

        // This forces a unique path every time so MAUI reloads it
        public ImageSource ProfileImageSource
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ProfileImageFile))
                {
                    var imagesFolder = AppPaths.GetImagesFolder();
                    var fullPath = Path.Combine(imagesFolder, ProfileImageFile);
                    if (File.Exists(fullPath))
                        return ImageSource.FromFile(fullPath + $"?{DateTime.Now.Ticks}");
                }
                return "icon.png";
            }
        }

        public string FullName => $"{FirstName} {LastName}";
        public string MaskedPassword =>
            string.IsNullOrEmpty(Password) ? "" :
            (Password.Length <= 3 ? new string('*', Password.Length)
                                  : Password[..3] + new string('*', Password.Length - 3));

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
