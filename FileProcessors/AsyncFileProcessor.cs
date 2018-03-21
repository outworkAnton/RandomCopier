using System.Collections.Generic;

namespace Sorter.FileProcessors
{
    class AsyncFileProcessor : BaseFileProcessor, IFileProcessor
    {

        public AsyncFileProcessor(FileProcessingOptions fileProcessingOptions) : base(fileProcessingOptions) { }

        public void ProcessData()
        {
            ProcessDataItems(CollectItems());
        }

        public override void ProcessDataItems(List<FileItem> items)
        {
            
        }
    }
}