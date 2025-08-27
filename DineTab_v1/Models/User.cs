using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.ComponentModel;
using System.IO;

namespace DineTab_v1.Models
{
    //model for user with property change notification for profile image updates
    public class User : INotifyPropertyChanged
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public byte[] ProfileImage { get; set; }

        // Property to get ImageSource from byte array
        public ImageSource ProfileImageSource
        {
            get
            {
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    return ImageSource.FromStream(() => new MemoryStream(ProfileImage));
                }
                return "icon.png"; // fallback avatar
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
