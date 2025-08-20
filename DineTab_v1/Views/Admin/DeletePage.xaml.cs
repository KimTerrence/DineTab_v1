using DineTab_v1.Models;
using DineTab_v1.ViewModels.Admin;
using System.Collections.ObjectModel;
using DineTab_v1.ViewModels;

namespace DineTab_v1.Views.Admin;

public partial class DeletePage : ContentPage
{
    public DeletePage(Item item, ObservableCollection<Item> menuItems)
    {
        InitializeComponent();
        BindingContext = new DeleteItemViewModel(item, menuItems);
    }
}
