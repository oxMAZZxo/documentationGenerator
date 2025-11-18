using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DocumentationGenerator.Views;

public partial class LanguageSelectionWindowView : Window
{
    public LanguageSelectionWindowView()
    {
        InitializeComponent();
    }

    public void OnOkayButtonClicked(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Continuing this operation.");
        this.Hide();
    }

    public void OnCancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Canceling this operation.");
        this.Hide();
    }

}