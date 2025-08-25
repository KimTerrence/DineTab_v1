using DineTab_v1.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;


namespace DineTab_v1.ViewModels.Auth
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private int step = 1;
        public int Step
        {
            get => step;
            set
            {
                if (step != value)
                {
                    step = value;
                    OnPropertyChanged(); // notify XAML
                }
            }
        }

        public string Email { get; set; }
        public string Pin { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand CancelCommand { get; }
        public ICommand ConfirmCommand { get; }
        private readonly AuthService _authService = new();

        public ForgotPasswordViewModel()
        {
            CancelCommand = new Command(OnCancel);
            ConfirmCommand = new Command(OnConfirm);
        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async void OnConfirm()
        {
            switch (Step)
            {
                case 1: // Send PIN to email
                    if (string.IsNullOrWhiteSpace(Email))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Please enter your email.", "OK");
                        return;
                    }

                    bool pinSent = await _authService.SendPinToEmailAsync(Email);
                    if (pinSent)
                    {
                        await Application.Current.MainPage.DisplayAlert("Success", "PIN sent to your email.", "OK");
                        Step = 2; // go to PIN entry
                        OnPropertyChanged(nameof(Step));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Email not found.", "OK");
                    }
                    break;

                case 2: // Verify PIN
                    if (string.IsNullOrWhiteSpace(Pin))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Please enter the PIN.", "OK");
                        return;
                    }

                    bool pinValid = await _authService.VerifyPinAsync(Email, Pin);
                    if (pinValid)
                    {
                        Step = 3; // go to new password entry
                        OnPropertyChanged(nameof(Step));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Invalid PIN.", "OK");
                    }
                    break;

                case 3: // Set new password
                    if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Please fill all fields.", "OK");
                        return;
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                        return;
                    }

                    // Password validation: min 8 chars + special character
                    if (NewPassword.Length < 8 || !Regex.IsMatch(NewPassword, @"[!@#$%^&*(),.?""':{}|<>]"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error",
                            "Password must be at least 8 characters and contain at least one special character.", "OK");
                        return;
                    }

                    bool updated = await _authService.ResetPasswordAsync(Email, NewPassword);
                    if (updated)
                    {
                        await Application.Current.MainPage.DisplayAlert("Success", "Password updated successfully!", "OK");
                        await Application.Current.MainPage.Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to update password.", "OK");
                    }
                    break;
            }
        }
    }
}
