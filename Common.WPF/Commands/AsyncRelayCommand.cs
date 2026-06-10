using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.WPF.Core.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _action;
        private readonly Func<object?, CancellationToken, Task>? _cancellableAction;
        private readonly Func<object?, bool>? _canExecute;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isExecuting;

        // Constructor for non-cancellable commands
        public AsyncRelayCommand(Func<object?, Task> action, Func<object?, bool>? canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(action);
            _action = action;
            _canExecute = canExecute;
        }

        // Constructor for cancellable commands
        public AsyncRelayCommand(Func<object?, CancellationToken, Task> action, Func<object?, bool>? canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(action);
            _cancellableAction = action;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        public event EventHandler<ExceptionEventArgs>? UnhandledException;
        public event EventHandler? Canceled;

        // Returns true if this command supports cancellation
        public bool CanCancel => _cancellableAction is not null;

        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting == value)
                {
                    return;
                }
                _isExecuting = value;
                NotifyCanExecuteChanged();
            }
        }

        public bool CanExecute(object? parameter) => !IsExecuting && (_canExecute is null || _canExecute(parameter));

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        // Explicit interface implementation - UI calls this
        async void ICommand.Execute(object? parameter) => await Execute(parameter);

        // Public Task-returning method - view models can await this
        public async Task Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                IsExecuting = true;

                // Non-cancellable execution
                if (_action is not null)
                {
                    await _action.Invoke(parameter);
                }

                // Cancellable execution
                if (_cancellableAction is not null)
                {
                    // Create a new cancellation token source for this execution
                    _cancellationTokenSource = new CancellationTokenSource();
                    await _cancellableAction.Invoke(parameter, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Operation was canceled - fire the Canceled event
                Canceled?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                var handler = UnhandledException;
                if (handler is null)
                {
                    throw;
                }
                handler.Invoke(this, new ExceptionEventArgs(ex));
            }
            finally
            {
                IsExecuting = false;
                // Clean up the token source
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        // Cancel the running operation
        public void Cancel()
        {
            if (!CanCancel)
            {
                throw new InvalidOperationException("This command does not support cancellation.");
            }
            _cancellationTokenSource?.Cancel();
        }

        public class ExceptionEventArgs(Exception exception) : EventArgs
        {
            public Exception Exception { get => exception; }
        }
    }

}
