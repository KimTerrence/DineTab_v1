using System.Windows.Input;
using Microsoft.Maui.Controls;
using DineTab_v1.Models;

namespace DineTab_v1.ViewModels.Admin
{
    public class DeleteViewModel : BaseViewModel
    {
        public Item TargetItem { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _category;
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private string _price;
        public string Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public ICommand CancelCommand { get; }
        public ICommand ConfirmDeleteCommand { get; }

        public event Action<Item> DeleteConfirmed;
        public event Action Cancelled;

        public DeleteViewModel(Item item)
        {
            TargetItem = item;
            Name = item.Name;
            Category = item.Category;
            Status = item.Status;
            Price = "Php " + " " + item.Price;

            CancelCommand = new Command(() => Cancelled?.Invoke());
            ConfirmDeleteCommand = new Command(() => DeleteConfirmed?.Invoke(item));
        }
    }
}
