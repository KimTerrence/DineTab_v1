using DineTab_v1.ViewModels.KitchenStaff;

namespace DineTab_v1.Views.KitchenStaff;

public partial class HistoryPage : ContentPage
{
	public HistoryPage()
	{
		InitializeComponent();
		BindingContext = new HistoryViewModel();
	}
}