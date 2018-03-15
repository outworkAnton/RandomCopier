using static Sorter.FileProcessors.FileProcessingOptions;
using System;


namespace Sorter.FileProcessors
{
    class FileProcessorFactory
    {
        private FileProcessingOptions _options;
        public FileProcessorFactory(FileProcessingOptions fileProcessingOptions)
        {
            _options = fileProcessingOptions;
        }

        public IFileProcessor CreateFileProcessor()
        {
            switch (_options.FileProcessingMode)
            {
                case FileProcessingModeEnum.Serial:
                    return new SerialFileProcessor(_options);
                case FileProcessingModeEnum.Task:
                    return new TPLFileProcessor(_options);
                case FileProcessingModeEnum.Action:
                    return new ActionFileProcessor(_options);
                case FileProcessingModeEnum.PLINQ:
                    return new PLINQFileProcessor(_options);
                case FileProcessingModeEnum.Async:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
