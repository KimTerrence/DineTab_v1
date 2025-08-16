using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Services;

namespace DineTab_v1.ViewModels.Admin
{
    public class ModifyCategoriesViewModel : BaseViewModel
    {
        public ObservableCollection<string> Categories => CategoryService.Instance.Categories;

        private string _newCategoryName;
        public string NewCategoryName
        {
            get => _newCategoryName;
            set { _newCategoryName = value; OnPropertyChanged(); }
        }

        public bool IsAddingCategory { get; set; } = false;
        public bool IsNotAddingCategory => !IsAddingCategory;

        public ICommand ShowAddCategoryCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand CancelAddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public ModifyCategoriesViewModel()
        {
            ShowAddCategoryCommand = new Command(() =>
            {
                IsAddingCategory = true;
                OnPropertyChanged(nameof(IsAddingCategory));
                OnPropertyChanged(nameof(IsNotAddingCategory));
            });

            AddCategoryCommand = new Command(() =>
            {
                if (string.IsNullOrWhiteSpace(NewCategoryName))
                {
                    Application.Current.MainPage.DisplayAlert("Warning", "Category name cannot be empty.", "OK");
                    return;
                }

                if (Categories.Contains(NewCategoryName))
                {
                    Application.Current.MainPage.DisplayAlert("Warning", "Category already exists.", "OK");
                    return;
                }

                Categories.Add(NewCategoryName);
                NewCategoryName = string.Empty;
                IsAddingCategory = false;
                OnPropertyChanged(nameof(IsAddingCategory));
                OnPropertyChanged(nameof(IsNotAddingCategory));
            });

            CancelAddCategoryCommand = new Command(() =>
            {
                NewCategoryName = string.Empty;
                IsAddingCategory = false;
                OnPropertyChanged(nameof(IsAddingCategory));
                OnPropertyChanged(nameof(IsNotAddingCategory));
            });

            DeleteCategoryCommand = new Command<string>(async (category) =>
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm", $"Are you sure you want to delete '{category}'?", "Yes", "No");

                if (confirm)
                    Categories.Remove(category);
            });

            SaveChangesCommand = new Command(async () =>
            {
                if (Categories.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Warning", "You must have at least one category.", "OK");
                    return;
                }

                await Application.Current.MainPage.DisplayAlert("Success", "Categories saved successfully!", "OK");

                // Notify parent to show MenuManagementPage
                MessagingCenter.Send(this, "BackToMenuManagement");
            });
        }
    }
}
