using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Item : INotifyPropertyChanged
{
    private string name;
    public string Name
    {
        get => name;
        set { name = value; OnPropertyChanged(); }
    }

    private string category;
    public string Category
    {
        get => category;
        set { category = value; OnPropertyChanged(); }
    }

    private string status;
    public string Status
    {
        get => status;
        set { status = value; OnPropertyChanged(); }
    }

    private string price;
    public string Price
    {
        get => price;
        set { price = value; OnPropertyChanged(); }
    }

    private string spicy;
    public string Spicy
    {
        get => spicy;
        set { spicy = value; OnPropertyChanged(); }
    }

    private string imagePath;
    public string ImagePath
    {
        get => imagePath;
        set { imagePath = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
