using Avalonia.Controls;
using Avalonia.Interactivity;
using DocumentationGenerator.ViewModels;

namespace DocumentationGenerator.Views;

public partial class SettingsWindowView : Window
{
    public SettingsWindowView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }

    private void OnCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
        this.Hide();
    }
}