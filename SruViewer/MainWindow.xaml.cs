namespace SruViewer;

using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

public partial class MainWindow : Window
{
    private ViewModel? viewModel;

    public MainWindow()
    {
        this.InitializeComponent();
    }

    private ViewModel ViewModel => this.viewModel ??= (ViewModel)this.DataContext;

    private void OnOpen(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            DefaultExt = ".sru",
            Filter = "SRU files|*.sru",
        };

        if (dialog.ShowDialog() is true)
        {
            this.ViewModel.Read(dialog.FileName);
        }
    }
}
