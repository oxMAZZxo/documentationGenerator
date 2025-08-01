using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace DocumentationGenerator.MVVM.Helpers
{
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
}
