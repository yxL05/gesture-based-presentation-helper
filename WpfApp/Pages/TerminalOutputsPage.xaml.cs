using System.Windows;
using System.Windows.Controls;

namespace HandyPresentationHelper.Pages
{
    public partial class TerminalOutputsPage : Page
    {
        public TerminalOutputsPage()
        {
            InitializeComponent();
        }

        // Expose a method to append text
        public void AppendOutput(string text)
        {
            TerminalOutputTextBox.AppendText(text + "\n");
            TerminalOutputTextBox.ScrollToEnd();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handle stopping the Python process or navigation
            MessageBox.Show("STOP button clicked!", "Stop Action");
        }
    }
}
