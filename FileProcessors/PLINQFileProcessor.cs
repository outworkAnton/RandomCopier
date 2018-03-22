using System.Collections.Generic;
using System.Linq;

namespace Sorter.FileProcessors
{
    public class PLINQFileProcessor : BaseFileProcessor
    {
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