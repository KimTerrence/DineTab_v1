using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DineTab_v1.Models;
using DineTab_v1.Views.Cashier;

public class PaymentViewModel : INotifyPropertyChanged
{
    private string _moneyReceived = "";
    private string _change = "";
    private string _amountToBePaid;
    private string _orderNumber;

    public string OrderNumber
    {
        get => _orderNumber;
        set { _orderNumber = value; OnPropertyChanged(); }
    }

    public string AmountToBePaid
    {
        get => _amountToBePaid;
        set { _amountToBePaid = value; OnPropertyChanged(); }
    }

    public string MoneyReceived
    {
        get => _moneyReceived;
        set
        {
            _moneyReceived = value;
            OnPropertyChanged();
            UpdateChange();
        }
    }

    public string Change
    {
        get => _change;
        set { _change = value; OnPropertyChanged(); }
    }

    // Commands
    public ICommand NumberCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand BackspaceCommand { get; }

    public ObservableCollection<OrderItem> OrderItems { get; set; }


    // 👉 Constructor now accepts OrderNumber
    public PaymentViewModel(string orderNumber, decimal amount, ObservableCollection<OrderItem> items)
    {
        OrderNumber = orderNumber;
        AmountToBePaid = amount.ToString("F2");

        NumberCommand = new Command<string>(OnNumberEntered);
        ClearCommand = new Command(OnClear);
        BackspaceCommand = new Command(OnBackspace);
        ConfirmCommand = new Command(async () => await OnConfirmAsync());
        OrderItems = items;
    }

    private void OnNumberEntered(string number)
    {
        MoneyReceived += number;
        UpdateChange();
    }

    private void OnClear()
    {
        MoneyReceived = "";
        Change = "";
    }

    private void OnBackspace()
    {
        if (!string.IsNullOrEmpty(MoneyReceived))
        {
            MoneyReceived = MoneyReceived.Substring(0, MoneyReceived.Length - 1);
            UpdateChange();
        }
    }

    private void UpdateChange()
    {
        if (decimal.TryParse(MoneyReceived, out decimal received) &&
            decimal.TryParse(AmountToBePaid, out decimal amount))
        {
            Change = (received - amount).ToString("F2");
        }
        else
        {
            Change = "";
        }
    }

    private async Task OnConfirmAsync()
    {
        string total = AmountToBePaid;

        await Application.Current.MainPage.Navigation.PushModalAsync(
            new OverviewPage(OrderNumber, OrderItems, total)
        );
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
