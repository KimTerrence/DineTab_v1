using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Admin
{
    public class AddNewItemViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public Item CurrentItem { get; set; }

        // Form fields
        public string ItemName { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string SelectedAvailability { get; set; } = "Available";
        public string SelectedSpicy { get; set; } = "No";
        public Category SelectedCategory { get; set; }
        public byte[]? ItemImage { get; set; }

        // Collections for UI
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<string> AvailabilityOptions { get; set; } = new() { "Available", "Unavailable" };
        public ObservableCollection<string> SpicyOptions { get; set; } = new() { "No", "Mild", "Spicy", "Extra Spicy" };

        // Commands
        public ICommand AddItemCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UploadImageCommand { get; }

        public AddNewItemViewModel(Item item = null)
        {
            // If item is provided, we are editing
            if (item != null)
            {
                CurrentItem = item;
                ItemName = item.ItemName;
                Price = item.Price.ToString();
                SelectedAvailability = item.Availability;
                SelectedSpicy = item.Spicy;
                SelectedCategory = new Category { Id = item.CategoryId, Name = item.CategoryName };
                ItemImage = item.Image;
            }
            else
            {
                CurrentItem = new Item();
            }

            // Commands
            AddItemCommand = new Command(async () => await AddOrUpdateItem());
            CancelCommand = new Command(async () => await Cancel());
            UploadImageCommand = new Command(async () => await UploadImage());

            // Load categories from database
            _ = LoadCategories();
        }

        private async Task LoadCategories()
        {
            var categories = await _dbService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categories)
                Categories.Add(cat);

            // Set SelectedCategory if editing
            if (CurrentItem.Id > 0)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == CurrentItem.CategoryId);
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private async Task AddOrUpdateItem()
        {
            if (string.IsNullOrWhiteSpace(ItemName) || string.IsNullOrWhiteSpace(Price))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill all required fields", "OK");
                return;
            }

            if (!decimal.TryParse(Price, out decimal priceValue))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid price", "OK");
                return;
            }

            CurrentItem.ItemName = ItemName;
            CurrentItem.Price = priceValue;
            CurrentItem.CategoryId = SelectedCategory?.Id ?? 0;
            CurrentItem.Availability = SelectedAvailability;
            CurrentItem.Spicy = SelectedSpicy;
            CurrentItem.Image = ItemImage;

            bool result;
            if (CurrentItem.Id > 0) // Existing item
            {
                result = await _dbService.UpdateMenuItemAsync(CurrentItem);
            }
            else // New item
            {
                result = await _dbService.AddMenuItemAsync(CurrentItem);
            }

            if (result)
            {
             
                await Application.Current.MainPage.DisplayAlert("Success", "Item saved successfully", "OK");                
                await Application.Current.MainPage.Navigation.PopModalAsync();
                MessagingCenter.Send(this, "MenuUpdated");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save item", "OK");
            }
        }

        private async Task Cancel()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async Task UploadImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select an image"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    ItemImage = ms.ToArray();
                    OnPropertyChanged(nameof(ItemImage));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
