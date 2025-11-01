using System;
using System.Windows.Input;

namespace DocumentationGenerator.Helpers;


#pragma warning disable
public class RelayCommand : ICommand
{
    private readonly Action execute;
    private readonly Func<bool> canExecute;

    public RelayCommand(Action newExecute, Func<bool> newCanExecute = null)
    {
        execute = newExecute;
        canExecute = newCanExecute;
    }

    public bool CanExecute(object parameter) => canExecute == null || canExecute();

    public void Execute(object parameter) => execute();

    public event EventHandler? CanExecuteChanged;
}
