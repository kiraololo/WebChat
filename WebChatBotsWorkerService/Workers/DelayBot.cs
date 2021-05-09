using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebChatBotsWorkerService.Workers
{
    public class DelayBot : IBot
    {
        public string BotName => "DelayBot";

        // Просто для примера спящий бот
        public IEnumerable<Func<CancellationToken, Task>> GetTasks(CancellationToken token)
         => new List<Func<CancellationToken, Task>> 
         {
            (token) => {
                Thread.Sleep(2000);
                return Task.CompletedTask; 
            }
         };
    }
}
