using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sorter.FileProcessors
{
    class TPLFileProcessor : BaseFileProcessor
    {
        private static void RunTaskQueue(Queue<Task> taskQueue)
        {
            var tasks = new List<Task>(Environment.ProcessorCount - 1);

            while (taskQueue.Any())
            {
                while (tasks.Count < tasks.Capacity)
                {
                    try
                    {
                        var task = taskQueue.Dequeue();
                        task.Start();
                        tasks.Add(task);
                    }
                    catch (InvalidOperationException)
                    {
                        Task.WaitAll(tasks.ToArray());
                        return;
                    }
                }

                tasks.RemoveAt(Task.WaitAny(tasks.ToArray()));
            }
        }

        public override void ProcessDataItems(List<FileItem> items)
        {
            var queue = new Queue<Task>(items.Select(item => new Task(() => ProcessItem(item))).ToList());
            RunTaskQueue(queue);
        }
    }
}
