using System.Collections.ObjectModel;
using System.ComponentModel;
using DineTab_v1.Models;
using DineTab_v1.Services;

public class NotificationViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _dbService = new();
    public ObservableCollection<NotificationItem> Notifications { get; set; } = new();

    public NotificationViewModel()
    {
        LoadNotifications();
    }

    private async void LoadNotifications()
    {
        var items = await _dbService.GetOrderNotificationsAsync();
        Notifications.Clear();
        foreach (var item in items)
        {
            Notifications.Add(item);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public string TodayDate => DateTime.Now.ToString("dddd, MMMM dd, yyyy");
}
