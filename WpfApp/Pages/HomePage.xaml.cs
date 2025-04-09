using System.Windows;
using System.Windows.Controls;

namespace HandyPresentationHelper.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OptionsPage());
        }
    }
}
