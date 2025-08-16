using DineTab_v1.Models;
using DineTab_v1.ViewModels.Admin;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace DineTab_v1.Views.Admin;

public partial class AddNewItemPage : ContentPage
{
    // Constructor for adding a new item
    public AddNewItemPage(ObservableCollection<Item> menuItems)
    {
        InitializeComponent();
        BindingContext = new AddNewItemViewModel(menuItems);
    }

    // Constructor for editing an existing item
    public AddNewItemPage(Item editingItem)
    {
        InitializeComponent();
        BindingContext = new AddNewItemViewModel(editingItem);
    }
}
