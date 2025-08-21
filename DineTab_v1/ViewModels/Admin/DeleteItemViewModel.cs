using DineTab_v1.Models;
using DineTab_v1.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace DineTab_v1.ViewModels.Admin
{
    public class DeleteItemViewModel : BaseViewModel
    {
        public Item SelectedItem { get; set; }
        private readonly DatabaseService _dbService = new DatabaseService();


        public DeleteItemViewModel(Item item, ObservableCollection<Item> menuItems)
        {
            ItemToDelete = item;
            MenuItems = menuItems;

            CancelCommand = new Command(OnCancel);
            ConfirmDeleteCommand = new Command(async () => await OnConfirmDelete());

            SelectedItem = item;

        }

      
        public Item ItemToDelete { get; set; }
        public ObservableCollection<Item> MenuItems { get; }

        public ICommand CancelCommand { get; }
        public ICommand ConfirmDeleteCommand { get; }

        private async void OnCancel()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task OnConfirmDelete()
        {
            if (ItemToDelete == null) return;

            bool deleted = await _dbService.DeleteMenuItemAsync(ItemToDelete.Id);

            if (deleted)
            {
                MessagingCenter.Send(this, "MenuUpdated"); // Notify MenuManagementPage to refresh
                MenuItems.Remove(ItemToDelete); // remove from UI list
                await Application.Current.MainPage.DisplayAlert("Success", "Item deleted", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete item", "OK");
            }

            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        // Bindable properties for XAML
        public string ItemName => SelectedItem?.ItemName;
        public string Price => SelectedItem?.Price.ToString("F2");
        public string Category => SelectedItem?.CategoryName;
        public string Status => SelectedItem?.Availability;
    }
}
