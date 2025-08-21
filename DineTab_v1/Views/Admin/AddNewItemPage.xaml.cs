using DineTab_v1.Models;
using DineTab_v1.ViewModels.Admin;
using Microsoft.Maui.Controls;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin
{
    [Preserve(AllMembers = true)]
    public partial class AddNewItemPage : ContentPage
    {
        // Parameterless constructor for XAML
        public AddNewItemPage() : this(null) { }

        // Constructor with Item parameter for editing
        public AddNewItemPage(Item item)
        {
            InitializeComponent();

            if (item == null)
            {
                BindingContext = new AddNewItemViewModel(); // New item
            }
            else
            {
                BindingContext = new AddNewItemViewModel(item); // Edit existing item
            }
        }
    }
}
