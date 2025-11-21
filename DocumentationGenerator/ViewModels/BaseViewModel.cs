using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;

namespace DocumentationGenerator.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    protected Window Owner {get;}

    public BaseViewModel(Window owner)
    {
        this.Owner = owner;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName]string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}