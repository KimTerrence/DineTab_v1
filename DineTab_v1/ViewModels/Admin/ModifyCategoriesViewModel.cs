using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Services;
using DineTab_v1.Models;

namespace DineTab_v1.ViewModels.Admin
{
    public class ModifyCategoriesViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public ModifyCategoriesViewModel()
        {
            Categories = new ObservableCollection<Category>();
            LoadCategories();

            ShowAddCategoryCommand = new Command(() => { IsAddingCategory = true; });
            CancelAddCategoryCommand = new Command(() =>
            {
                IsAddingCategory = false;
                NewCategoryName = "";
            });
            AddCategoryCommand = new Command(async () => await AddCategory());
            DeleteCategoryCommand = new Command<Category>(async (cat) => await DeleteCategory(cat));
            SaveChangesCommand = new Command(async () => await SaveChanges());
        }

        public ObservableCollection<Category> Categories { get; set; }

        private bool _isAddingCategory = false;
        public bool IsAddingCategory
        {
            get => _isAddingCategory;
            set
            {
                _isAddingCategory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotAddingCategory));
            }
        }
        public bool IsNotAddingCategory => !IsAddingCategory;

        public string NewCategoryName { get; set; }

        // Commands
        public ICommand ShowAddCategoryCommand { get; }
        public ICommand CancelAddCategoryCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand SaveChangesCommand { get; }

        private async Task LoadCategories()
        {
            var categories = await _dbService.GetCategoriesAsync(); // returns List<Category>
            Categories.Clear();
            foreach (var cat in categories)
                Categories.Add(cat);
        }

        private async Task AddCategory()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName)) return;

            // Add category to database and get new ID
            int newId = await _dbService.AddCategoryAsync(NewCategoryName); // make this return the inserted ID
            if (newId > 0)
            {
                Categories.Add(new Category { Id = newId, Name = NewCategoryName });
                NewCategoryName = "";
                IsAddingCategory = false;
            }
        }

        private async Task DeleteCategory(Category category)
        {
            if (category == null) return;

            bool deleted = await _dbService.DeleteCategoryAsync(category.Id);
            if (deleted)
            {
                Categories.Remove(category);
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync(); //close modal
            }
            catch (Exception ex) { }

            MessagingCenter.Send(this, "CategoriesUpdated");
        }
    }
}
