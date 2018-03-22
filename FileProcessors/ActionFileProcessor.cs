using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sorter.FileProcessors
{
    class ActionFileProcessor : BaseFileProcessor
    {
        public override void ProcessDataItems(List<FileItem> items)
        {
            var actions = new List<Action>();
            items.AsParallel().Select(item => {
                actions.Add(() => ProcessItem(item));
                return item;
                }).ToList();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 };
            Parallel.Invoke(options, actions.ToArray());
        }
    }
}
