using System.Windows.Input;
using Avalonia.Controls;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Views;

namespace DocumentationGenerator.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private Window owner;
    public ICommand SettingsButtonCommand { get; set; }
    private SettingsWindowView settingsView;

    public MainWindowViewModel(Window owner)
    {
        this.owner = owner;
        settingsView = new SettingsWindowView();
        SettingsButtonCommand = new RelayCommand(OnSettingsButtonClicked);
    }

    private void OnSettingsButtonClicked()
    {
        settingsView.ShowDialog(owner);
    }

}
