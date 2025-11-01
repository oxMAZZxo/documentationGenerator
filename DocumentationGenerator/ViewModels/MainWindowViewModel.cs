using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using DocumentationGenerator.Helpers;

namespace DocumentationGenerator.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public ICommand CloseButtonCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName]string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public MainWindowViewModel()
    {
        CloseButtonCommand = new RelayCommand(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked()
    {
        Environment.Exit(0);
    }
}
