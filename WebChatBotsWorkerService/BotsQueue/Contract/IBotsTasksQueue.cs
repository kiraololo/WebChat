using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebChatBotsWorkerService.BotsQueue.Contract
{
    public interface IBotsTasksQueue
    {
        int Size { get; }

        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
