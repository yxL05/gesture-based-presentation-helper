using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Text.Json;

namespace HandyPresentationHelper.Pages
{
    public partial class OptionsPage : Page
    {
        const string PPT_PATH = @"C:\Program Files\Microsoft Office\root\Office16\POWERPNT.EXE";
        const string WORD_PATH = @"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE";

        public OptionsPage()
        {
            InitializeComponent();
            LoadGestures();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected app name from dropdown
            string? selectedApp = (AppComboBox.SelectedItem as ComboBoxItem)?.Content as string;

            if (string.IsNullOrEmpty(selectedApp))
            {
                MessageBox.Show("Please select an app first.", "No App Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Make sure all actions are mapped to gestures
            if (ToggleGestureComboBox.SelectedItem == null ||
                StartGestureComboBox.SelectedItem == null ||
                DownGestureComboBox.SelectedItem == null ||
                UpGestureComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a gesture for all keybinds (Toggle, Start, Go Down, Go Up).", "Incomplete Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // No duplicate gestures allowed
            var selectedGestures = new[]
            {
                ToggleGestureComboBox.SelectedItem?.ToString(),
                StartGestureComboBox.SelectedItem?.ToString(),
                DownGestureComboBox.SelectedItem?.ToString(),
                UpGestureComboBox.SelectedItem?.ToString()
            };

            if (selectedGestures.Distinct().Count() != selectedGestures.Length)
            {
                MessageBox.Show("Each gesture must be assigned to only one keybind. Please ensure all keybinds are unique.", "Duplicate Keybinds", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Map UI selection to actual process name
            string? processName = selectedApp switch
            {
                "PowerPoint" => "POWERPNT",  // PowerPoint.exe
                "Word" => "WINWORD",         // Word.exe
                _ => null
            };

            if (string.IsNullOrEmpty(processName))
            {
                MessageBox.Show("Unknown application selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if the process is running
            var runningProcesses = Process.GetProcessesByName(processName);

            if (runningProcesses.Any())
            {
                MessageBox.Show($"{selectedApp} is running!", "App Found", MessageBoxButton.OK, MessageBoxImage.Information);

                // Create and navigate to TerminalOutputsPage
                var terminalPage = new TerminalOutputsPage();
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(terminalPage);

                // Proceed to WebSocket connection or other startup tasks here
                LaunchPythonBackend(terminalPage);

            }
            else
            {
                MessageBox.Show($"{selectedApp} is NOT running. Attempting to launch it now.", "App Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);

                try
                {
                    Process? startedProcess = null;

                    if (processName == "POWERPNT")
                    {
                        startedProcess = Process.Start(PPT_PATH);
                    }
                    else if (processName == "WINWORD")
                    {
                        startedProcess = Process.Start(WORD_PATH);
                    }
                    else
                    {
                        MessageBox.Show("Cannot launch the selected application automatically.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (startedProcess != null)
                    {
                        startedProcess.WaitForInputIdle(5000);  // Wait up to 5 seconds for the app to become idle
                    }

                    MessageBox.Show($"{selectedApp} launched successfully!", "App Launched", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to launch {selectedApp}: {ex.Message}", "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

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
                        string gesture = line.Trim();

                        ToggleGestureComboBox.Items.Add(gesture);
                        StartGestureComboBox.Items.Add(gesture);
                        DownGestureComboBox.Items.Add(gesture);
                        UpGestureComboBox.Items.Add(gesture);
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

        public class GestureMessage
        {
            public string Gesture { get; set; }
            public string Timestamp { get; set; }
        }

        private async Task ConnectToGestureWebSocketAsync(TerminalOutputsPage terminalPage)
        {
            using var ws = new ClientWebSocket();
            var buffer = new byte[1024];

            try
            {
                await ws.ConnectAsync(new Uri("ws://localhost:8765"), CancellationToken.None);
                Dispatcher.Invoke(() => terminalPage.AppendOutput("[INFO] Connected to gesture WebSocket server"));

                while (ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    try
                    {
                        var msg = JsonSerializer.Deserialize<GestureMessage>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        Dispatcher.Invoke(() =>
                        {
                            terminalPage.AppendOutput($"[GESTURE] {msg.Gesture} @ {msg.Timestamp}");

                            // TODO: Control PowerPoint based on msg.Gesture here
                            // if (msg.Gesture == "Swipe Right") new PowerPointController().NextSlide();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            terminalPage.AppendOutput($"[ERROR] Failed to parse JSON: {ex.Message}");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    terminalPage.AppendOutput($"[ERROR] WebSocket connection failed: {ex.Message}");
                });
            }
        }

        private async void LaunchPythonBackend(TerminalOutputsPage terminalPage)
        {
            try
            {
                // Bunch of constants for .venv Python path
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\.."));

                string pythonExePath = Path.Combine(solutionRoot, @"hand-gesture-recognition-mediapipe-main\.venv\Scripts\python.exe");
                string scriptPath = Path.Combine(solutionRoot, @"hand-gesture-recognition-mediapipe-main\app.py");

                // TODO: insert correct args
                string selfieModeArg = (SelfieModeCheckBox.IsChecked == true) ? "--selfie_mode 1" : "--selfie_mode 0";

                // Sum of args
                string additionalArgs = selfieModeArg;

                var psi = new ProcessStartInfo
                {
                    FileName = pythonExePath,
                    Arguments = $"-u \"{scriptPath}\" {additionalArgs}",
                    WorkingDirectory = solutionRoot,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var process = new Process
                {
                    StartInfo = psi
                };

                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            terminalPage.AppendOutput($"[STDOUT] {e.Data}");
                        });
                    }
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            terminalPage.AppendOutput($"[STDERR] {e.Data}");
                        });
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await Task.Delay(2000); // Give Python some time to start the server
                _ = ConnectToGestureWebSocketAsync(terminalPage); // fire and forget

                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start Python script: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
