using System.Windows;
using ZPLStudio.ViewModels;

namespace ZPLStudio;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
