using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CampusRoster.ViewModels;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Raise([CallerMemberName] string? nama = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nama));

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? nama = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        Raise(nama);
        return true;
    }
}
