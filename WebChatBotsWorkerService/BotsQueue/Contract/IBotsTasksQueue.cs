using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebChatBotsWorkerService.BotsQueue.Contract
{
    public interface IBotsTasksQueue
    {
        int Size { get; }

        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        
        void QueueBackgroundWorkItems(IEnumerable<Func<CancellationToken, Task>> workItems);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
