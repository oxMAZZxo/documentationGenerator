using System.ComponentModel;

namespace DocumentationGenerator.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public event PropertyChangedEventHandler? PropertyChanged;
}
