using Avalonia.Controls;
using Avalonia.Input;
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

    private void OnChromeBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    private void OnChromeBarDoubleTap(object? sender, TappedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            
        }else if(WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
    }
}