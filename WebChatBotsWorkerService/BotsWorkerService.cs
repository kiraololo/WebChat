using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebChatBotsWorkerService.BotsQueue.Contract;
using WebChatBotsWorkerService.BotsQueue.Implementation;

namespace WebChatBotsWorkerService
{
    public class BotsWorkerService : BackgroundService
    {
        private readonly IServiceProvider services;
        public BotsWorkerService(IServiceProvider services)
        {
            this.services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var workers = new List<Task>();
            
            var workersCount = 1;

            var angryBotTasksQueue = services.GetRequiredService<AngryBotTasksQueue>();
            var angryBotWorkers = Enumerable.Range(0, workersCount).Select(num => RunInstance(num, angryBotTasksQueue, token));
            workers.AddRange(angryBotWorkers);

            var commandBotTasksQueue = services.GetRequiredService<CommandBotTasksQueue>();
            var commandBotWorkers = Enumerable.Range(0, workersCount).Select(num => RunInstance(num, commandBotTasksQueue, token));
            workers.AddRange(commandBotWorkers);

            var urlBotTasksQueue = services.GetRequiredService<UrlBotTasksQueue>();
            var urlBotWorkers = Enumerable.Range(0, workersCount).Select(num => RunInstance(num, urlBotTasksQueue, token));
            workers.AddRange(urlBotWorkers);

            await Task.WhenAll(workers);
        }

        private async Task RunInstance(int idx, IBotsTasksQueue tasksQueue, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var workItem = await tasksQueue.DequeueAsync(token);

                try
                {
                    await workItem(token);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
