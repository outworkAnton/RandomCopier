using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sorter.FileProcessors
{
    class ActionFileProcessor : BaseFileProcessor, IFileProcessor
    {
        private List<Action> actions = new List<Action>();

        public ActionFileProcessor(FileProcessingOptions fileProcessingOptions) : base(fileProcessingOptions) { }

        public void ProcessData()
        {
            ProcessDataItems(CollectItems());
        }

        public override void ProcessDataItems(List<FileItem> items)
        {
            items.AsParallel().Select(item => {
                actions.Add(() => ProcessItem(item));
                return item;
                }).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 };
            Parallel.Invoke(options, actions.ToArray());
        }
    }
}
