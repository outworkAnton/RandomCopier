using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Sorter.FileProcessor;
using Application = System.Windows.Application;

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
            set => Dispatcher.Invoke(() => { Progress.Maximum = value; }, DispatcherPriority.Background);
        }

        internal double foldersCnt
        {
            get => Dispatcher.Invoke(() => Convert.ToDouble(FoldCnt.Content), DispatcherPriority.Background);
            set => Dispatcher.Invoke(() => { FoldCnt.Content = value; }, DispatcherPriority.Background);
        }

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var mode = MoveMode.IsChecked != null && MoveMode.IsChecked.Value ? Mode.Move : Mode.Copy;
            var sourceDirectory = SourceDirectory.Text;
            var alsoFromSubfolders = IncludeSubDirs.IsChecked;
            var targetDirectory = string.IsNullOrWhiteSpace(TargetDirectory.Text) ? null : TargetDirectory.Text;
            var newFolderName = NewFolderName.Text;
            var newFolderPostfix = string.IsNullOrWhiteSpace(NewFolderPostfix.Text) ? 1 : Convert.ToInt32(NewFolderPostfix.Text);
            var countFilesPerFolder = string.IsNullOrWhiteSpace(FilesPerFolder.Text) ? 0 : Convert.ToInt32(FilesPerFolder.Text);
            var processMethod = (ProcessMethod) Enum.Parse(typeof(ProcessMethod), FilesPresort.Text, true);
            var renameMode = (RenameMode) Enum.Parse(typeof(RenameMode), RenameMode.Text.Replace(" ", ""), true);
            var renameSymbols = Symbols.Text;
            var span = new TimeSpan();

            var perf = new Stopwatch();
            perf.Start();
            Progress.Visibility = Visibility.Visible;

            var taskInThread = Task.Factory.StartNew(() =>
            {
                new FileProcessor.FileProcessor(mode, sourceDirectory, alsoFromSubfolders,
                    targetDirectory, newFolderName, newFolderPostfix, countFilesPerFolder,
                    processMethod, renameMode, renameSymbols).ProcessData();
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