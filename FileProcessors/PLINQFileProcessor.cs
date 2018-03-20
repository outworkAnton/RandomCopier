using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Sorter.FileProcessors.FileProcessingOptions;

namespace Sorter.FileProcessors
{
    class PLINQFileProcessor : IFileProcessor
    {
        private readonly FileProcessingOptions _options;
        private int _fileStartIndex;
        private List<Action> actions = new List<Action>();
        private readonly object _itemObj = new object();
        private readonly object _foldObj = new object();

        public PLINQFileProcessor(FileProcessingOptions fileProcessingOptions)
        {
            _options = fileProcessingOptions;
        }

        class FileItem
        {
            public FileInfo OriginalFile { get; }
            public string DestinationFile { get; }

            public FileItem(FileInfo origFile, string destFile)
            {
                OriginalFile = origFile;
                DestinationFile = destFile;
            }
        }

        public void ProcessData()
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
                    filesQueue.Add(new FileItem(_options.FilesList[i],folderName + "\\" + newFileName));
                }

                _fileStartIndex += _options.CountFilesPerFolder;
            }

            filesQueue.AsParallel().Select(item =>
            {
                ProcessItem(item);
                return item;
            }).ToList();
        }

        private void ProcessItem(FileItem fileItem)
        {
            var dirOfDestFile = new FileInfo(fileItem.DestinationFile).Directory;
            if (!dirOfDestFile.Exists)
            {
                Directory.CreateDirectory(dirOfDestFile.FullName);
                lock (_foldObj)
                {
                    MainWindow.mainWindow.foldersCnt++;
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

            lock (_itemObj)
            {
                MainWindow.mainWindow.progress++;
            }
        }
    }
}