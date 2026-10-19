using System.Windows;
using CampusRoster.ViewModels;

namespace CampusRoster;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
