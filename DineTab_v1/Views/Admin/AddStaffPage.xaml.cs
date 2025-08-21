using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin
{
    [Preserve(AllMembers = true)]
    public partial class AddStaffPage : ContentPage
    {
        public AddStaffPage()
        {
            InitializeComponent();
            BindingContext = new AddStaffViewModel();
        }
    }
}
