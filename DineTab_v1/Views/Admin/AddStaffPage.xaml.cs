using DineTab_v1.ViewModels.Admin;

namespace DineTab_v1.Views.Admin
{
    public partial class AddStaffPage : ContentPage
    {
        public AddStaffPage()
        {
            InitializeComponent();
            BindingContext = new AddStaffViewModel();
        }
    }
}
