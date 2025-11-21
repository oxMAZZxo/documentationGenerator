using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DocumentationGenerator.Models;

namespace DocumentationGenerator.Views;

public partial class LanguageSelectionWindowView : Window
{
    public ProgLanguage SelectedProgrammingLanguage { get; private set; }
    public bool ValidSelection { get; private set; }

    public LanguageSelectionWindowView()
    {
        InitializeComponent();
        ProgLangOptions.ItemsSource = Enum.GetValues(typeof(ProgLanguage));
        ValidSelection = false;
    }

    public void OnOkayButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (ProgLangOptions.SelectedIndex > -1)
        {
            ValidSelection = true;
        }
        SelectedProgrammingLanguage = (ProgLanguage)ProgLangOptions.SelectedIndex;
        this.Hide();
    }

    public void OnCancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        ValidSelection = false;
        this.Hide();
    }
}