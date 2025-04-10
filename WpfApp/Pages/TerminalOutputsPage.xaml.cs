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
    }
}
