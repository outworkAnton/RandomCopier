using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Sorter.FileProcessors.FileProcessingOptions;

namespace Sorter.FileProcessors
{
    class TPLFileProcessor : IFileProcessor
    {
        private readonly FileProcessingOptions _options;
        private int _fileStartIndex;
        private Queue<Task> taskQueue = new Queue<Task>();
        private readonly object _itemObj = new object();
        private readonly object _foldObj = new object();

        public TPLFileProcessor(FileProcessingOptions fileProcessingOptions)
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

                taskQueue.Enqueue(new Task(() => ProcessItem(filesQueue, folderName)));

                _fileStartIndex += _options.CountFilesPerFolder;
            }

            RunTaskQueue(taskQueue);
        }

        private static void RunTaskQueue(Queue<Task> taskQueue)
        {
            var tasks = new List<Task>(Environment.ProcessorCount - 1);

            while (taskQueue.Any())
            {
                while (tasks.Count < tasks.Capacity)
                {
                    try
                    {
                        var task = taskQueue.Dequeue();
                        task.Start();
                        tasks.Add(task);
                    }
                    catch (InvalidOperationException)
                    {
                        Task.WaitAll(tasks.ToArray());
                        return;
                    }
                }

                Task.WaitAny(tasks.ToArray());
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

                lock (_itemObj)
                {
                    MainWindow.mainWindow.progress++;
                }
            }

            lock (_foldObj)
            {
                MainWindow.mainWindow.foldersCnt++;
            }
        }
    }
}
