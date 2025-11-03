using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace DocumentationGenerator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void OnWindowDoubleTap(object? sender, TappedEventArgs e)
    {
        MaximiseWindow();
    }

    // private void OnMaximiseButtonClicked(object? sender, RoutedEventArgs e)
    // {
    //     MaximiseWindow();
    // }
    
    private void MaximiseWindow()
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        } else if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
    }

    // private void OnMinimiseButtonClicked(object? sender, RoutedEventArgs e)
    // {
    //     WindowState = WindowState.Minimized;
    // }


}