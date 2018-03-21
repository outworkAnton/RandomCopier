using static Sorter.FileProcessors.FileProcessingOptions;
using System;

namespace Sorter.FileProcessors
{
    static class FileProcessorFactory
    {
        public static IFileProcessor CreateFileProcessor(FileProcessingOptions fileProcessingOptions)
        {
            switch (fileProcessingOptions.FileProcessingMode)
            {
                case FileProcessingModeEnum.Serial:
                    return new SerialFileProcessor(fileProcessingOptions);
                case FileProcessingModeEnum.Task:
                    return new TPLFileProcessor(fileProcessingOptions);
                case FileProcessingModeEnum.Action:
                    return new ActionFileProcessor(fileProcessingOptions);
                case FileProcessingModeEnum.PLINQ:
                    return new PLINQFileProcessor(fileProcessingOptions);
                case FileProcessingModeEnum.Async:
                    return new AsyncFileProcessor(fileProcessingOptions);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
