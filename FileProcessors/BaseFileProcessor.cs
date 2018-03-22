using System.Collections.Generic;
using System.IO;
using static Sorter.FileProcessors.FileProcessingOptions;

namespace Sorter.FileProcessors
{
    public abstract class BaseFileProcessor : IFileProcessor
    {
        protected FileProcessingOptions _options;
        private int _fileStartIndex;
        private readonly object _itemObj = new object();
        private readonly object _foldObj = new object();
        public double ProgressCounter
        {
            get { return MainWindow.mainWindow.progress; }
            set { MainWindow.mainWindow.progress = value; }
        }
        public double FolderCounter
        {
            get { return MainWindow.mainWindow.foldersCnt; }
            set { MainWindow.mainWindow.foldersCnt = value; }
        }

        public virtual void ProcessData(FileProcessingOptions fileProcessingOptions)
        {
            _options = fileProcessingOptions;
            ProcessDataItems(CollectItems());
        }

        private List<FileItem> CollectItems()
        {
            var filesQueue = new List<FileItem>();
            for (var folderIndex = 0; folderIndex < _options.FoldersCount; folderIndex++)
            {
                var folderName = _options.TargetDirectory + "\\" + _options.NewFolderName + " " +
                                 (_options.NewFolderPostfix + folderIndex);
                for (var i = _fileStartIndex; i < (_fileStartIndex + _options.CountFilesPerFolder); i++)
                {
                    var newFileName = _options.GetFileName(i);
                    if (newFileName == null) break;
                    filesQueue.Add(new FileItem(_options.FilesList[i], folderName + "\\" + newFileName));
                }

                _fileStartIndex += _options.CountFilesPerFolder;
            }
            return filesQueue;
        }

        public abstract void ProcessDataItems(List<FileItem> items);

        protected virtual void ProcessItem(FileItem fileItem)
        {
            var dirOfDestFile = new FileInfo(fileItem.DestinationFile).Directory;
            if (!dirOfDestFile.Exists)
            {
                Directory.CreateDirectory(dirOfDestFile.FullName);
                lock (_foldObj)
                {
                    FolderCounter++;
                }
            }
            switch (_options.FileManipulationMode)
            {
                case FileManipulationModeEnum.Copy:
                    fileItem.OriginalFile.CopyTo(fileItem.DestinationFile);
                    break;
                case FileManipulationModeEnum.Move:
                    fileItem.OriginalFile.MoveTo(fileItem.DestinationFile);
                    break;
            }

            fileItem.Dispose();

            lock (_itemObj)
            {
                ProgressCounter++;
            }
        }
    }
}