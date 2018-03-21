using System.Collections.Generic;

namespace Sorter.FileProcessors
{
    public class SerialFileProcessor : BaseFileProcessor, IFileProcessor
    {
        public SerialFileProcessor(FileProcessingOptions fileProcessingOptions) : base(fileProcessingOptions) { }

        public void ProcessData()
        {
            ProcessDataItems(CollectItems());
        }

        public override void ProcessDataItems(List<FileItem> items)
        {
            foreach (var item in items)
            {
                ProcessItem(item);
            }
        }
    }
}
