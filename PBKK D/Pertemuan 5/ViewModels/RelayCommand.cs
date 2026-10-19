using System.Windows.Input;

namespace CampusRoster.ViewModels;

public class RelayCommand : ICommand
{
    private readonly Action _aksi;
    private readonly Func<bool>? _bolehJalan;

    public RelayCommand(Action aksi, Func<bool>? bolehJalan = null)
    {
        _aksi = aksi;
        _bolehJalan = bolehJalan;
    }

    public bool CanExecute(object? parameter) => _bolehJalan?.Invoke() ?? true;
    public void Execute(object? parameter) => _aksi();

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
