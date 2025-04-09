using System.Windows;
using System.Windows.Navigation;

namespace HandyPresentationHelper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.HomePage());
        }
    }
}
