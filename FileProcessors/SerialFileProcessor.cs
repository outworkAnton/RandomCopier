using System.Collections.Generic;
using System.IO;
using static Sorter.FileProcessors.FileProcessingOptions;

namespace Sorter.FileProcessors
{
    class SerialFileProcessor : IFileProcessor
    {
        private readonly FileProcessingOptions _options;
        private int _fileStartIndex;

        public SerialFileProcessor(FileProcessingOptions fileProcessingOptions)
        {
            _options = fileProcessingOptions;
        }

        public void ProcessData()
        {
            for (var folderIndex = 0; folderIndex < _options.FoldersCount; folderIndex++)
            {
                var folderName = _options.TargetDirectory + "\\" + _options.NewFolderName + " " +
                                 (_options.NewFolderPostfix + folderIndex);
                var filesQueue = new Dictionary<FileInfo, string>(_options.CountFilesPerFolder);
                for (var i = _fileStartIndex; i < (_fileStartIndex + _options.CountFilesPerFolder); i++)
                {
                    var newFileName = _options.GetFileName(i);
                    if (newFileName == null) break;
                    filesQueue.Add(_options.FilesList[i], newFileName);
                }
                ProcessItem(filesQueue, folderName);

                _fileStartIndex += _options.CountFilesPerFolder;
            }
        }

        private void ProcessItem(Dictionary<FileInfo, string> queue, string folderName)
        {
            var newDir = Directory.CreateDirectory(folderName).FullName;
            foreach (var item in queue)
            {
                var destFilename = newDir + "\\" + item.Value;
                switch (_options.FileManipulationMode)
                {
                    case FileManipulationModeEnum.Copy:
                        item.Key.CopyTo(destFilename);
                        break;
                    case FileManipulationModeEnum.Move:
                        item.Key.MoveTo(destFilename);
                        break;
                }
                    MainWindow.mainWindow.progress++;
            }
                MainWindow.mainWindow.foldersCnt++;
        }
    }
}