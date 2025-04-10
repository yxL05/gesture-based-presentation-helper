using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace HandyPresentationHelper.Pages
{
    public partial class OptionsPage : Page
    {
        public OptionsPage()
        {
            InitializeComponent();
            LoadGestures();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add action when GO! is clicked
            MessageBox.Show("Starting Presentation Helper...", "GO!");
        }
        private void LoadGestures()
        {
            try
            {
                // Path to your CSV file
                string csvPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            @"..\..\..\..\hand-gesture-recognition-mediapipe-main\model\keypoint_classifier\keypoint_classifier_label.csv");

                if (File.Exists(csvPath))
                {
                    var lines = File.ReadAllLines(csvPath);

                    foreach (var line in lines)
                    {
                        // Add each line as a ComboBox item
                        GestureComboBox.Items.Add(line.Trim());
                    }
                }
                else
                {
                    MessageBox.Show($"Could not find {csvPath}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading gestures: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
