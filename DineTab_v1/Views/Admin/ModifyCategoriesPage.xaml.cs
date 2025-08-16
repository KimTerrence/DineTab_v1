using DineTab_v1.ViewModels.Admin;

namespace DineTab_v1.Views.Admin;

public partial class ModifyCategoriesPage : ContentView
{
    public ModifyCategoriesPage()
    {
        InitializeComponent();
        BindingContext = new ModifyCategoriesViewModel();
    }
}
