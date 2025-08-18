using DineTab_v1.ViewModels.Admin;

namespace DineTab_v1.Views.Admin;

public partial class ModifyCategoriesPage : ContentPage
{
    public ModifyCategoriesPage()
    {
        InitializeComponent();
        BindingContext = new ModifyCategoriesViewModel();
    }
}
