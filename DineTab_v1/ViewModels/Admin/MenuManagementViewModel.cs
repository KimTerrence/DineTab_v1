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

        private readonly DatabaseService _dbService = new DatabaseService(); //database service instance


        public ObservableCollection<Item> MenuItems { get; set; } = new();

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
        public ObservableCollection<Category> Categories { get; set; } = new();

        public ObservableCollection<Item> Items { get; set; }
        public ICommand LoadDataCommand { get; }

        public MenuManagementViewModel()
        {
            MessagingCenter.Subscribe<DeleteItemViewModel>(this, "MenuUpdated", async (sender) =>
            {
                await LoadMenuItemsAsync();
            });

            MessagingCenter.Subscribe<ModifyCategoriesViewModel>(this, "CategoriesUpdated", async (sender) =>
            {
                await LoadCategories();
            });

         
            MenuItems = new ObservableCollection<Item>(); //

            ModifyCategoriesCommand = new Command(OnModifyCategories);
            OpenAddItemPageCommand = new Command(OnOpenAddItemPage);
            EditItemCommand = new Command<Item>(OnEditItem);
            DeleteItemCommand = new Command<Item>(OnDeleteItem);

            // Load initial data
            LoadDataCommand = new Command(async () => await LoadData());
            LoadDataCommand.Execute(null);
        }
        //
        private async Task LoadData()
        {
            // Load categories from database
            var categoriesFromDb = await _dbService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categoriesFromDb)
                Categories.Add(cat);

        }

        private async void OnModifyCategories()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ModifyCategoriesPage());
        }

        private async void OnOpenAddItemPage()
        {
            // Navigate to AddNewItemPage
            var page = new Views.Admin.AddNewItemPage();
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

        private async void OnDeleteItem(Item item)
        {
            if (item == null) return;

            var deletePage = new Views.Admin.DeletePage(item, MenuItems);
            await Application.Current.MainPage.Navigation.PushModalAsync(deletePage);
        }


        // Load categories from database
        private async Task LoadCategories()
        {
            var categoriesFromDb = await _dbService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categoriesFromDb)
                Categories.Add(cat);
        }

        //Load menu items from database
        public async Task LoadMenuItemsAsync()
        {
            var items = await _dbService.GetMenuItemsAsync();
            MenuItems.Clear();
            foreach (var item in items)
                MenuItems.Add(item);
        }

        // Edit item command handler
        private async void OnEditItem(Item item)
        {
            if (item == null) return;

            // Navigate to Add/Edit Item page with the selected item
            var editPage = new Views.Admin.AddNewItemPage(item);
            await Application.Current.MainPage.Navigation.PushModalAsync(editPage);
        }

    }
}
