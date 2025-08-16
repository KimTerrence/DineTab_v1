using System.Collections.ObjectModel;

namespace DineTab_v1.Services
{
    public class CategoryService
    {
        private static CategoryService _instance;
        public static CategoryService Instance => _instance ??= new CategoryService();

        public ObservableCollection<string> Categories { get; set; }

        private CategoryService()
        {
            Categories = new ObservableCollection<string>
            {
                "Food",
                "Drinks",
                "Desserts"
            };
        }
    }
}
