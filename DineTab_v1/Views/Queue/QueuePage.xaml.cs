using DineTab_v1.ViewModels.Queue;

namespace DineTab_v1.Views.Queue;

public partial class QueuePage : ContentPage
{
    private QueueViewModel _viewModel;

    public QueuePage()
    {
        InitializeComponent();
        _viewModel = new QueueViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Load data from database
        _viewModel.LoadDataCommand.Execute(null);

        // Start the live countdown timer
        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            foreach (var order in _viewModel.PreparingOrders)
            {
                if (order.PreparingUntil.HasValue)
                {
                    var remaining = order.PreparingUntil.Value - DateTime.Now;
                    order.RemainingTime = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
                }
            }
            return true; // repeat every second
        });
    }

}
