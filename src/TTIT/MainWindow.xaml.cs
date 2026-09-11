// Ignore Spelling: TTIT

using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TTIT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;

            this.Closing += MainWindow_Closing;

            // Load settings
            HoursBox.Text = Properties.Settings.Default.Hours.ToString();
            MinutesBox.Text = Properties.Settings.Default.Minutes.ToString();
        }



        private async void OnScheduleShutdownClick(object sender, RoutedEventArgs e)
        {
            await StartShutdownAsync();
        }

        private async void CancelShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            await CancelShutdownAsync();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Hours = int.TryParse(HoursBox.Text, out int hours) ? hours : 0;
            Properties.Settings.Default.Minutes = int.TryParse(MinutesBox.Text, out int minutes) ? minutes : 0;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Start scheduled shutdown asynchronously.
        /// </summary>
        private async Task StartShutdownAsync()
        {
            // First, send a cancel command to make sure any previous shutdown is cancelled
            await SendCommandAsync(command: "/a");

            // Wait for 10 seconds
            await Task.Delay(TimeSpan.FromSeconds(2));

            // Now, set up the new shutdown
            _ = int.TryParse(HoursBox.Text, out int hours);
            _ = int.TryParse(MinutesBox.Text, out int minutes);

            int totalSeconds = hours * 3600 + minutes * 60;

            if (totalSeconds <= 0)
                return;

            string shutdownCmd = $"-s -t {totalSeconds}";
            await SendCommandAsync(shutdownCmd);
        }


        /// <summary>
        /// Cancel scheduled shutdown.
        /// </summary>
        private static async Task CancelShutdownAsync()
        {
            await SendCommandAsync("/a");
        }

        private static async Task SendCommandAsync(string command)
        {
            ProcessStartInfo? processStartInfo = new("shutdown", command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using Process? process = Process.Start(processStartInfo);

            if(process != null)
            {
                await process.WaitForExitAsync();
            }
        }


        private void TitleBar_HelpClicked(Wpf.Ui.Controls.TitleBar sender, RoutedEventArgs args)
        {
            HelpWindow gifWindow = new();
            gifWindow.Show();
        }
    }

}