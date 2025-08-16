using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Admin
{
    public class AddNewItemViewModel : BaseViewModel
    {
        public ObservableCollection<string> Categories => CategoryService.Instance.Categories;
        public ObservableCollection<string> AvailabilityOptions { get; } = new() { "Available", "Out of Stock" };
        public ObservableCollection<string> SpicyOptions { get; } = new() { "Yes", "No" };

        // Editable fields
        public string ItemName { get; set; }
        public string Price { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedAvailability { get; set; }
        public string SelectedSpicy { get; set; }
        public string ItemImage { get; set; }

        public ICommand AddItemCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UploadImageCommand { get; }

        private readonly Item _editingItem;
        private readonly ObservableCollection<Item> _menuItems;

        // Constructor for adding a new item
        public AddNewItemViewModel(ObservableCollection<Item> menuItems)
        {
            _menuItems = menuItems;

            AddItemCommand = new Command(OnAddItem);
            CancelCommand = new Command(OnCancel);
            UploadImageCommand = new Command(OnUploadImage);
        }

        // Constructor for editing an existing item
        public AddNewItemViewModel(Item editingItem)
        {
            _editingItem = editingItem;

            ItemName = editingItem.Name;
            Price = editingItem.Price;
            SelectedCategory = editingItem.Category;
            SelectedAvailability = editingItem.Status;
            SelectedSpicy = editingItem.Spicy;
            ItemImage = editingItem.ImagePath;

            AddItemCommand = new Command(OnEditItem);
            CancelCommand = new Command(OnCancel);
            UploadImageCommand = new Command(OnUploadImage);
        }

        private async void OnAddItem()
        {
            if (string.IsNullOrWhiteSpace(ItemName) || string.IsNullOrWhiteSpace(SelectedCategory))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Item name and category cannot be empty.", "OK");
                return;
            }

            _menuItems.Add(new Item
            {
                Name = ItemName,
                Category = SelectedCategory,
                Status = SelectedAvailability,
                Spicy = SelectedSpicy,
                Price = Price,
                ImagePath = ItemImage
            });

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void OnEditItem()
        {
            if (_editingItem != null)
            {
                _editingItem.Name = ItemName;
                _editingItem.Category = SelectedCategory;
                _editingItem.Status = SelectedAvailability;
                _editingItem.Spicy = SelectedSpicy;
                _editingItem.Price = Price;
                _editingItem.ImagePath = ItemImage;

                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void OnUploadImage()
        {
            // implement image picker here
        }
    }
}
