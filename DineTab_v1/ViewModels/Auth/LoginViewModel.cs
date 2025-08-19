using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;
using DineTab_v1.Views.KitchenStaff;
using DineTab_v1.Views.Cashier;

namespace DineTab_v1.ViewModels.Auth
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService = new();

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }

        public INavigation Navigation { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLogin);

        }

        private async void OnLogin()
        {
            //var user = _authService.Login("admin", "1234");
            var user = _authService.Login(Email, Password);

            if (user == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid credentials", "OK");
                return;
            }

            switch (user.Role)
            {
                case "Admin":
                    await Navigation.PushAsync(new AdminPage(user));
                    break;
                case "Staff":
                    await Navigation.PushAsync(new KitchenStaffPage());
                    break;
                case "Cashier":
                    await Navigation.PushAsync(new CashierMenuPage());
                    break;
            }
        }
    }
}