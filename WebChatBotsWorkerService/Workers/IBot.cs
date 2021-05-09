using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebChatBotsWorkerService.Workers
{
    public interface IBot
    {
        public string BotName { get; }

        public IEnumerable<Func<CancellationToken, Task>> GetTasks(CancellationToken token);
    }
}
