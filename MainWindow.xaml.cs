using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using Sorter.FileProcessors;
using Application = System.Windows.Application;
using static Sorter.FileProcessors.FileProcessingOptions;
using System.Windows.Input;

namespace Sorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        internal static MainWindow mainWindow;

        internal double progress
        {
            get => Dispatcher.Invoke(() => Progress.Value, DispatcherPriority.Background);
            set => Dispatcher.Invoke(() =>
            {
                Progress.Value = value;
                FilesProcCount.Content = value;
            }, DispatcherPriority.Background);
        }

        internal double max
        {
            set => Dispatcher.Invoke(() => Progress.Maximum = value, DispatcherPriority.Background);
        }

        internal double foldersCnt
        {
            get => Dispatcher.Invoke(() => Convert.ToDouble(FoldCnt.Content), DispatcherPriority.Background);
            set => Dispatcher.Invoke(() => FoldCnt.Content = value, DispatcherPriority.Background);
        }

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var options = new FileProcessingOptions(
                (FileProcessingModeEnum)Enum.Parse(typeof(FileProcessingModeEnum), FileProcessing.Text, true),
                MoveMode.IsChecked != null && MoveMode.IsChecked.Value ? FileManipulationModeEnum.Move : FileManipulationModeEnum.Copy,
                SourceDirectory.Text,
                IncludeSubDirs.IsChecked,
                string.IsNullOrWhiteSpace(TargetDirectory.Text) ? null : TargetDirectory.Text,
                NewFolderName.Text,
                string.IsNullOrWhiteSpace(NewFolderPostfix.Text) ? 1 : Convert.ToInt32(NewFolderPostfix.Text),
                string.IsNullOrWhiteSpace(FilesPerFolder.Text) ? 0 : Convert.ToInt32(FilesPerFolder.Text),
                (PresortMethodEnum)Enum.Parse(typeof(PresortMethodEnum), FilesPresort.Text, true),
                (RenameModeEnum)Enum.Parse(typeof(RenameModeEnum), RenameMode.Text.Replace(" ", ""), true),
                Symbols.Text);
            var span = new TimeSpan();

            var perf = new Stopwatch();
            perf.Start();
            Progress.Visibility = Visibility.Visible;

            var taskInThread = Task.Factory.StartNew(() =>
            {
                new FileProcessorFactory(options).CreateFileProcessor().ProcessData();
            });
            while (!taskInThread.IsCompleted)
            {
                Application.Current.DoEvents();
                span = perf.Elapsed;
                TimeElapsed.Content = $"{span.Minutes} min {span.Seconds} sec";
            }

            Progress.Visibility = Visibility.Collapsed;
            perf.Stop();
            span = perf.Elapsed;
            TimeElapsed.Content = $"{span.Minutes} min {span.Seconds} sec";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SelectSource_Click(object sender, RoutedEventArgs e)
        {
            SourceDirectory.Text = OpenFolderDialog();
        }

        private void SelectTarget_Click(object sender, RoutedEventArgs e)
        {
            TargetDirectory.Text = OpenFolderDialog();
        }

        private string OpenFolderDialog()
        {
            using (var dlg = new FolderBrowserDialog())
            {
                return dlg.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK ? dlg.SelectedPath : null;
            }
        }
    }
}