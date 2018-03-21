using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sorter.FileProcessors
{
    public class FileProcessingOptions
    {
        public FileProcessingModeEnum FileProcessingMode { get; }
        public FileManipulationModeEnum FileManipulationMode { get; }
        public string SourceDirectory { get; }
        public bool? AlsoFromSubfolders { get; }
        public string TargetDirectory { get; }
        public string NewFolderName { get; }
        public int NewFolderPostfix { get; }
        public int CountFilesPerFolder { get; }
        public PresortMethodEnum PresortMethod { get; }
        public RenameModeEnum RenameMode { get; }
        public string RenameSymbols { get; }
        public List<FileInfo> FilesList { get; }
        public int FoldersCount { get; }

        public FileProcessingOptions(FileProcessingModeEnum fileProcessingMode, FileManipulationModeEnum fileManipulationMode, string sourceDirectory, bool? alsoFromSubfolders, string targetDirectory,
            string newFolderName,
            int newFolderPostfix, int countFilesPerFolder, PresortMethodEnum presortMethod, RenameModeEnum renameMode,
            string renameSymbols)
        {
            FileProcessingMode = fileProcessingMode;
            FileManipulationMode = fileManipulationMode;
            SourceDirectory = (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
                ? throw new DirectoryNotFoundException()
                : sourceDirectory;
            AlsoFromSubfolders = alsoFromSubfolders ?? false;
            TargetDirectory = targetDirectory ?? sourceDirectory;
            NewFolderName = string.IsNullOrWhiteSpace(newFolderName) ? "New folder" : newFolderName;
            NewFolderPostfix = newFolderPostfix;
            PresortMethod = presortMethod;
            RenameMode = renameMode;
            RenameSymbols = renameSymbols;
            FilesList = GetFilesList();
            CountFilesPerFolder = countFilesPerFolder == 0 ? FilesList.Count : countFilesPerFolder;
            FoldersCount = (int)Math.Ceiling((double)FilesList.Count / CountFilesPerFolder);
            MainWindow.mainWindow.max = FilesList.Count;
        }

        public string GetFileName(int i)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(FilesList[i].FullName);
                var ext = FilesList[i].Extension;
                if (RenameMode != RenameModeEnum.None)
                {
                    switch (RenameMode)
                    {
                        case RenameModeEnum.StartWith:
                            fileName = RenameSymbols + fileName;
                            break;
                        case RenameModeEnum.EndWith:
                            fileName += RenameSymbols;
                            break;
                        case RenameModeEnum.Numeric:
                            var format = new string('0', FilesList.Count.ToString().Length);
                            fileName = i.ToString(format);
                            break;
                    }
                }

                return fileName + ext;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<FileInfo> GetFilesList()
        {
            var files = new DirectoryInfo(SourceDirectory).GetFiles("*",
                AlsoFromSubfolders.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            switch (PresortMethod)
            {
                case PresortMethodEnum.Random:
                    var rand = new Random();
                    return files.OrderBy(delegate { return rand.Next(); }).ToList();
                case PresortMethodEnum.Alphabetically:
                    return files.OrderBy(info => info.Name).ToList();
                case PresortMethodEnum.BySize:
                    return files.OrderBy(info => info.Length).ToList();
            }

            return new List<FileInfo>();
        }

        public enum FileManipulationModeEnum
        {
            Copy,
            Move
        }

        public enum PresortMethodEnum
        {
            Random,
            Alphabetically,
            BySize
        }

        public enum RenameModeEnum
        {
            None,
            StartWith,
            EndWith,
            Numeric
        }

        public enum FileProcessingModeEnum
        {
            Serial,
            Task,
            Action,
            PLINQ,
            Async
        }
    }
}
