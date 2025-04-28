using System.ComponentModel;
using Avalonia.Threading;

public class ClientTable : INotifyPropertyChanged
{





    private string _requestedRecipe;
    public string RequestedRecipe
    {
        get => _requestedRecipe;
        set { _requestedRecipe = value; OnPropertyChanged(nameof(RequestedRecipe)); }
    }

    private bool _isWaiting;
    public bool IsWaiting
    {
        get => _isWaiting;
        set { _isWaiting = value; OnPropertyChanged(nameof(IsWaiting)); }
    }

    private bool _isServed;
    public bool IsServed
    {
        get => _isServed;
        set { _isServed = value; OnPropertyChanged(nameof(IsServed)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        });
    }
}
