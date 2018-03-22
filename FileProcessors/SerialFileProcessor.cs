using System.Collections.Generic;

namespace Sorter.FileProcessors
{
    public class SerialFileProcessor : BaseFileProcessor
    {
        public override void ProcessDataItems(List<FileItem> items)
        {
            foreach (var item in items)
            {
                ProcessItem(item);
            }
        }
    }
}
