using System.Windows.Input;

namespace DineTab_v1.ViewModels.Admin
{
    public class AddStaffViewModel : BaseViewModel
    {
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public AddStaffViewModel()
        {
            CancelCommand = new Command(OnCancel);
            SaveCommand = new Command(OnSave);
        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async void OnSave()
        {
            // TODO: Save staff logic here (DB, API, etc.)
            await Application.Current.MainPage.DisplayAlert("Success", "Staff saved!", "OK");
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
