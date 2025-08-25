using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Services;

namespace DineTab_v1.Views.Admin
{
    public partial class ReportsPage : ContentView
    {
        public ReportsPage()
        {
            InitializeComponent();
            this.BindingContext = new ReportsViewModel(); 
        }
    }

}
