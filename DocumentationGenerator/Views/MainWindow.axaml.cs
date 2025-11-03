using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace DocumentationGenerator.Views;

public partial class MainWindow : Window
{
    private bool windowLostFocus;

    public MainWindow()
    {
        InitializeComponent();
        FilePopup.Opened += OnFilePopupOpened;
        ExportPopup.Opened += OnExportPopupOpened;

        windowLostFocus = false;
        this.LostFocus += OnWindowLostFocus;
        this.PointerEntered += OnWindowPointerEntered;
    }

    private void OnMenuButtonPointerOver(object? sender, RoutedEventArgs e)
    {
        if(FilePopup.IsOpen && ExportPopup.IsOpen)
        {
            ExportPopup.Close();
            FilePopup.Focus();
        }
    }

    private void OnExportPopupOpened(object? sender, EventArgs e)
    {
        ExportPopup.Focus();
    }

    private void OnWindowPointerEntered(object? sender, PointerEventArgs e)
    {
        if(!windowLostFocus) { return; }
        if(FilePopup.IsOpen && windowLostFocus)
        {
            FilePopup.Close();
            this.Focus();
        }
    }

    private void OnWindowLostFocus(object? sender, RoutedEventArgs e)
    {
        windowLostFocus = true;
    }

    private void OnFilePopupOpened(object? sender, EventArgs e)
    {
        FilePopup.Focus();
    }

    private void OnFileButtonClicked(object? sender, RoutedEventArgs e)
    {
        FilePopup.Open();
    }

    private void OnExportButtonClicked(object? sender, RoutedEventArgs e)
    {
        ExportPopup.Open();
    }

}