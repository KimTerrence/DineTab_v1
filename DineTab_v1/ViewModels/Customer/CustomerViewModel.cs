using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace DineTab_v1.ViewModels.Customer
{
    public class CustomerViewModel : BindableObject
    {
        // Commands for the buttons
        public ICommand DineInCommand { get; }
        public ICommand TakeOutCommand { get; }
        public ICommand CancelCommand { get; }

        public CustomerViewModel()
        {
            DineInCommand = new Command(OnDineIn);
            TakeOutCommand = new Command(OnTakeOut);
            CancelCommand = new Command(OnCancel);
        }

        private async void OnDineIn()
        {
            Application.Current.MainPage = new NavigationPage(new Views.Customer.CustomerMenuPage());
        }

        private async void OnTakeOut()
        {
          
            Application.Current.MainPage = new NavigationPage(new Views.Customer.CustomerMenuPage());
        }

        private async void OnCancel()
        {
            // Navigate back to home or previous page
            //await Shell.Current.GoToAsync("//HomePage"); // adjust to your home route
            Application.Current.MainPage = new NavigationPage(new Views.Auth.LoginPage());
        }
    }
}
