using DineTab_v1.ViewModels.Admin;
using DineTab_v1.Models;

namespace DineTab_v1.Views.Admin
{
    public partial class DeletePage : ContentPage
    {
        public DeletePage(Item item, Action<Item> onConfirmed)
        {
            InitializeComponent();

            var vm = new DeleteViewModel(item);
            BindingContext = vm;

            vm.DeleteConfirmed += async (deletedItem) =>
            {
                onConfirmed?.Invoke(deletedItem);
                if (Navigation.ModalStack.Count > 0)
                    await Navigation.PopModalAsync();
            };

            vm.Cancelled += async () =>
            {
                if (Navigation.ModalStack.Count > 0)
                    await Navigation.PopModalAsync();
            };
        }
    }
}
