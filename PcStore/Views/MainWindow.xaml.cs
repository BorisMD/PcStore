using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace PcStore
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public MainWindow()
        {
            // XAML uses this
        }

    }
}