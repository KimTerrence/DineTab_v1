using System.Collections.ObjectModel;
using System.Windows.Input;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;
using DineTab_v1.Views.Customer;
using DineTab_v1.Views.Cashier;
using DineTab_v1.Views.KitchenStaff;

namespace DineTab_v1.ViewModels.Auth
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService = new();

        public ObservableCollection<string> Roles { get; } = new()
        {
            "Admin", "Cashier", "Kitchen Staff", "Customer", "Que"
        };

        private string selectedRole;
        public string SelectedRole
        {
            get => selectedRole;
            set
            {
                if (SetProperty(ref selectedRole, value))
                {
                    UpdateUIForRole();
                }
            }
        }

        private bool isLoginFormVisible = true;
        public bool IsLoginFormVisible
        {
            get => isLoginFormVisible;
            set => SetProperty(ref isLoginFormVisible, value);
        }

        private bool isCustomerSelected;
        public bool IsCustomerSelected
        {
            get => isCustomerSelected;
            set => SetProperty(ref isCustomerSelected, value);
        }

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand OrderCommand { get; }

        private INavigation _navigation;

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;

            LoginCommand = new Command(OnLogin);
            OrderCommand = new Command(OnOrder);
        }

        private void UpdateUIForRole()
        {
            IsLoginFormVisible = SelectedRole == "Admin" || SelectedRole == "Cashier" || SelectedRole == "Kitchen Staff";
            IsCustomerSelected = SelectedRole == "Customer";

            if (SelectedRole == "Que")
            {
                //_navigation.PushAsync(new QuePage());
            }
        }

        private async void OnLogin()
        {
            try
            {
                var user = await _authService.LoginAsync(Email, Password);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid credentials", "OK");
                    return;
                }

                // Role restriction logic
                if (SelectedRole == "Admin" && user.Role != "Admin")
                {
                    await Application.Current.MainPage.DisplayAlert("Access Denied", "You are not Admin", "OK");
                    return;
                }

                if (SelectedRole == "Cashier" && user.Role != "Cashier")
                {
                    await Application.Current.MainPage.DisplayAlert("Access Denied", "You are not Cashier", "OK");
                    return;
                }

                if (SelectedRole == "Kitchen Staff" && user.Role != "Kitchen Staff")
                {
                    await Application.Current.MainPage.DisplayAlert("Access Denied", "You are not Kitchen Staff", "OK");
                    return;
                }

                // Navigate based on actual user role
                switch (user.Role)
                {
                    case "Admin":
                        await _navigation.PushAsync(new AdminPage(user));
                        break;
                    case "Kitchen Staff":
                        await _navigation.PushAsync(new KitchenStaffPage());
                        break;
                    case "Cashier":
                        await _navigation.PushAsync(new CashierMenuPage());
                        break;
                    default:
                        await Application.Current.MainPage.DisplayAlert("Error", "Unknown role", "OK");
                        break;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        }


        private async void OnOrder()
        {
            await _navigation.PushAsync(new CustomerPage());
        }
    }
}
