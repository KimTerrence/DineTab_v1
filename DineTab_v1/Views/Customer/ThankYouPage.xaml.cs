using System;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using DineTab_v1.Views.Auth;

namespace DineTab_v1.Views.Customer
{
    public partial class ThankYouPage : ContentPage
    {
        public ThankYouPage()
        {
            InitializeComponent();
            RedirectAfterDelay();
        }

        private async void RedirectAfterDelay()
        {
            await Task.Delay(10000); // 10 seconds
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
