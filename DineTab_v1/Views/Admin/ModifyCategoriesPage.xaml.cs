using DineTab_v1.ViewModels.Admin;

using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin;

public partial class ModifyCategoriesPage : ContentPage
{
    [Preserve(AllMembers = true)]
    public ModifyCategoriesPage()
    {
        InitializeComponent();
        BindingContext = new ModifyCategoriesViewModel();
    }
}
