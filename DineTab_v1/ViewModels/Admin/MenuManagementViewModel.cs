using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;


namespace DineTab_v1.ViewModels.Admin
{
    public class MenuManagementViewModel : BaseViewModel
    {
        public ObservableCollection<string> Categories => CategoryService.Instance.Categories;
        public ObservableCollection<Item> MenuItems { get; set; }

        // Inputs for new item
        private string _newItemName;
        public string NewItemName
        {
            get => _newItemName;
            set { _newItemName = value; OnPropertyChanged(); }
        }

        private string _newItemCategory;
        public string NewItemCategory
        {
            get => _newItemCategory;
            set { _newItemCategory = value; OnPropertyChanged(); }
        }

        public ICommand ModifyCategoriesCommand { get; }
        public ICommand OpenAddItemPageCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand DeleteItemCommand { get; }

        public MenuManagementViewModel()
        {
            MenuItems = new ObservableCollection<Item>
            {
                new Item { Name="Burger", Category="Food", Status="Available" },
                new Item { Name="Pizza", Category="Food", Status="Out of Stock" },
                new Item { Name="Coke", Category="Drinks", Status="Available" },
                new Item { Name="Ice Cream", Category="Desserts", Status="Available" }
            };

            ModifyCategoriesCommand = new Command(OnModifyCategories);
            OpenAddItemPageCommand = new Command(OnOpenAddItemPage);
            EditItemCommand = new Command<Item>(OnEditItem);
            DeleteItemCommand = new Command<Item>(OnDeleteItem);
        }

        private async void OnModifyCategories()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ModifyCategoriesPage());
        }

        private async void OnOpenAddItemPage()
        {
            // Navigate to AddNewItemPage
            var page = new Views.Admin.AddNewItemPage(MenuItems);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

        private async void OnEditItem(Item item)
        {
            if (item == null) return;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Admin.AddNewItemPage(item));
        }


        private async void OnDeleteItem(Item item)
        {
            if (item == null) return;

            var page = new Views.Admin.DeletePage(item, (confirmedItem) =>
            {
                if (MenuItems.Contains(confirmedItem))
                    MenuItems.Remove(confirmedItem);
            });

            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

    }
}
