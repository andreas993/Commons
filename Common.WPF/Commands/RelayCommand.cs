using System;
using System.Windows.Input;

namespace Common.WPF.Core.Commands
{
    public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) : ICommand
    {
        private readonly Action<object?> _execute = execute;
        private readonly Predicate<object?>? _canExecute = canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? param) =>
            _canExecute?.Invoke(param) ?? true;

        public void Execute(object? param) => _execute(param);
    }

}
