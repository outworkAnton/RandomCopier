using static Sorter.FileProcessors.FileProcessingOptions;
using System;

namespace Sorter.FileProcessors
{
    static class FileProcessorFactory
    {
        public static IFileProcessor CreateFileProcessor(FileProcessingModeEnum fileProcessingMode)
        {
            switch (fileProcessingMode)
            {
                case FileProcessingModeEnum.Serial:
                    return new SerialFileProcessor();
                case FileProcessingModeEnum.Task:
                    return new TPLFileProcessor();
                case FileProcessingModeEnum.Action:
                    return new ActionFileProcessor();
                case FileProcessingModeEnum.PLINQ:
                    return new PLINQFileProcessor();
                case FileProcessingModeEnum.Async:
                    return new AsyncFileProcessor();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
