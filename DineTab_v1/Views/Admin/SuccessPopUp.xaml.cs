using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace DineTab_v1.Views.Admin
{
    public partial class SuccessPopUp : ContentPage
    {
        public SuccessPopUp()
        {
            InitializeComponent();
            CloseAfterDelay();
        }

        private async void CloseAfterDelay()
        {
            await Task.Delay(3000); // wait for 3 seconds
            await Navigation.PopModalAsync(); // go back to previous page
        }
    }
}
