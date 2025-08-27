using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;
using DineTab_v1.Services;
using DineTab_v1.Views.Admin;
using System.Globalization;

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

        //search
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                ApplySearchAndCategoryFilter();    // 🔹 call filter
            }
        }

        private string _selectedCategory = "All Items"; // default
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged();
                ApplySearchAndCategoryFilter();   // filter when category changes
            }
        }

        public ICommand ModifyCategoriesCommand { get; }
        public ICommand OpenAddItemPageCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ObservableCollection<Category> Categories { get; set; } = new();
        private ObservableCollection<Item> _allMenuItems = new(); // keep all items unfiltered

        public ObservableCollection<Item> Items { get; set; }
        public ICommand LoadDataCommand { get; }

        public MenuManagementViewModel()
        {
            MessagingCenter.Subscribe<DeleteItemViewModel>(this, "MenuUpdated", async (sender) =>
            {
                await LoadMenuItemsAsync();
            });

            MessagingCenter.Subscribe<AddNewItemViewModel>(this, "MenuUpdated", async (sender) =>
            {
                await LoadMenuItemsAsync();
            });

            MessagingCenter.Subscribe<ModifyCategoriesViewModel>(this, "CategoriesUpdated", async (sender) =>
            {
                await LoadCategories();
            });

            SelectCategoryCommand = new Command<string>(cat =>
            {
                SelectedCategory = cat;
            });

            MenuItems = new ObservableCollection<Item>(); //

            ModifyCategoriesCommand = new Command(async () => await OnModifyCategories());
            OpenAddItemPageCommand = new Command(async () => await OnOpenAddItemPage());
            EditItemCommand = new Command<Item>(async (item) => await OnEditItem(item));
            DeleteItemCommand = new Command<Item>(async (item) => await OnDeleteItem(item));

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

        private async Task OnModifyCategories()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ModifyCategoriesPage());
        }

        private async Task OnOpenAddItemPage()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddNewItemPage());
        }

        private async Task OnDeleteItem(Item item)
        {
            if (item == null) return;

            var deletePage = new Views.Admin.DeletePage(item, MenuItems);
            await Application.Current.MainPage.Navigation.PushModalAsync(deletePage);
        }

        // Load categories from database
        private async Task LoadCategories()
        {
            var categoriesFromDb = await _dbService.GetCategoriesAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Categories.Clear();
                foreach (var cat in categoriesFromDb)
                    Categories.Add(cat);
            });
        }

        public async Task LoadMenuItemsAsync()
        {
            var items = await _dbService.GetMenuItemsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _allMenuItems.Clear();
                MenuItems.Clear();

                foreach (var item in items)
                {
                    _allMenuItems.Add(item);
                    MenuItems.Add(item);
                }
            });
        }

        // Edit item command handler
        private async Task OnEditItem(Item item)
        {
            if (item == null) return;

            // Navigate to Add/Edit Item page with the selected item
            var editPage = new Views.Admin.AddNewItemPage(item);
            await Application.Current.MainPage.Navigation.PushModalAsync(editPage);
        }

        private void ApplySearchAndCategoryFilter()
        {
            IEnumerable<Item> filtered = _allMenuItems;

            // Apply category filter (skip if "All Items")
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All Items")
            {
                filtered = filtered.Where(i => i.CategoryName.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(i => i.ItemName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Update UI collection
            MenuItems.Clear();
            foreach (var item in filtered)
                MenuItems.Add(item);
        }
    }

    // Converter to highlight only selected category
    public class CategoryToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedCategory = value as string;
            string thisCategory = parameter as string;

            if (selectedCategory == thisCategory)
                return Colors.Orange; // highlight selected

            return Colors.Transparent; // default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    


    }
    // Converter inside the same namespace
    public class CategorySelectionToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Colors.White;

            string selectedCategory = values[0] as string;
            string thisCategory = values[1] as string;

            if (string.IsNullOrEmpty(selectedCategory) || string.IsNullOrEmpty(thisCategory))
                return Colors.White;

            return selectedCategory == thisCategory ? Colors.Orange : Colors.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
