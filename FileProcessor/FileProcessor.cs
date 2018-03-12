using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sorter.FileProcessor
{
    public class FileProcessor
    {
        private Mode Mode { get; }
        private string SourceDirectory { get; }
        private bool? AlsoFromSubfolders { get; }
        private string TargetDirectory { get; }
        private string NewFolderName { get; }
        private int NewFolderPostfix { get; }
        private int CountFilesPerFolder { get; }
        private ProcessMethod ProcessMethod { get; }
        private RenameMode RenameMode { get; }
        private string RenameSymbols { get; }
        private List<FileInfo> FilesList { get; }
        private TimeSpan span;

        public FileProcessor(Mode mode, string sourceDirectory, bool? alsoFromSubfolders, string targetDirectory,
            string newFolderName,
            int newFolderPostfix, int countFilesPerFolder, ProcessMethod processMethod, RenameMode renameMode,
            string renameSymbols)
        {
            Mode = mode;
            SourceDirectory = (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
                ? throw new DirectoryNotFoundException()
                : sourceDirectory;
            AlsoFromSubfolders = alsoFromSubfolders ?? false;
            TargetDirectory = targetDirectory ?? sourceDirectory;
            NewFolderName = string.IsNullOrWhiteSpace(newFolderName) ? "New folder" : newFolderName;
            NewFolderPostfix = newFolderPostfix;
            ProcessMethod = processMethod;
            RenameMode = renameMode;
            RenameSymbols = renameSymbols;
            FilesList = GetFilesList();
            CountFilesPerFolder = countFilesPerFolder == 0 ? FilesList.Count : countFilesPerFolder;
        }

        public void ProcessData()
        {
            MainWindow.mainWindow.max = FilesList.Count;
            var foldersCount = Math.Ceiling((double)FilesList.Count / CountFilesPerFolder);
            var fileStartIndex = 0;
            List<Task> tasks = new List<Task>();
            for (var folderIndex = 0; folderIndex < foldersCount; folderIndex++)
            {
                var newDir =
                    Directory.CreateDirectory(TargetDirectory + "\\" + NewFolderName + " " +
                                              (NewFolderPostfix + folderIndex));
                var filesQueue = new Dictionary<FileInfo, string>();
                for (var i = fileStartIndex; i < (fileStartIndex + CountFilesPerFolder); i++)
                {
                    var fileName = GetFileName(i);
                    if (fileName == null) break;
                    filesQueue.Add(FilesList[i], newDir.FullName + "\\" + fileName);
                }

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    ProcessItem(filesQueue);
                }));

                fileStartIndex += CountFilesPerFolder;
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void ProcessItem(Dictionary<FileInfo, string> queue)
        {
            foreach (var item in queue)
            {
                switch (Mode)
                {
                    case Mode.Copy:
                        item.Key.CopyTo(item.Value);
                        break;
                    case Mode.Move:
                        item.Key.MoveTo(item.Value);
                        break;
                }

                MainWindow.mainWindow.progress++;
            }
        }

        private string GetFileName(int i)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(FilesList[i].FullName);
                var ext = FilesList[i].Extension;
                if (RenameMode != RenameMode.None)
                {
                    switch (RenameMode)
                    {
                        case RenameMode.StartWith:
                            fileName = RenameSymbols + fileName;
                            break;
                        case RenameMode.EndWith:
                            fileName += RenameSymbols;
                            break;
                        case RenameMode.Numeric:
                            var format = new string('0', FilesList.Count.ToString().Length);
                            fileName = i.ToString(format);
                            break;
                    }
                }

                return fileName + ext;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private List<FileInfo> GetFilesList()
        {
            var files = new DirectoryInfo(SourceDirectory).GetFiles("*",
                (AlsoFromSubfolders.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
            switch (ProcessMethod)
            {
                case ProcessMethod.Random:
                    var rand = new Random();
                    return files.OrderBy(info => rand.Next()).ToList();
                case ProcessMethod.Alphabetically:
                    return files.OrderBy(info => info.Name).ToList();
                case ProcessMethod.BySize:
                    return files.OrderBy(info => info.Length).ToList();
            }

            return new List<FileInfo>();
        }
    }

    public enum Mode
    {
        Copy,
        Move
    }

    public enum ProcessMethod
    {
        Random,
        Alphabetically,
        BySize
    }

    public enum RenameMode
    {
        None,
        StartWith,
        EndWith,
        Numeric
    }
}