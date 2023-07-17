using System.Windows;
using System.Windows.Input;
using JupiterBachLabelPrinter.ViewModels;

namespace JupiterBachLabelPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow( MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 
                || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9
                || e.Key == Key.Delete
                || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }

        private void IpTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 
                || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 
                || e.Key == Key.OemPeriod 
                || e.Key == Key.Delete 
                || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }
    }
}
