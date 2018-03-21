using System.Collections.Generic;
using System.Linq;

namespace Sorter.FileProcessors
{
    public class PLINQFileProcessor : BaseFileProcessor, IFileProcessor
    {
        public PLINQFileProcessor(FileProcessingOptions fileProcessingOptions) : base(fileProcessingOptions) { }

        public void ProcessData()
        {
            ProcessDataItems(CollectItems());
        }

        public override void ProcessDataItems(List<FileItem> items)
        {
            items.AsParallel().Select(item =>
            {
                ProcessItem(item);
                return item;
            }).ToList();
        }
    }
}