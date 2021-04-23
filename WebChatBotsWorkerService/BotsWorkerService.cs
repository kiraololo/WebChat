using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BotsWorkerService> logger;
        public BotsWorkerService(IServiceProvider services, ILogger<BotsWorkerService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var workers = new List<Task>();
            
            var angryBotWorkersCount = 1;
            var commandBotWorkersCount = 1;
            var urlBotWorkersCount = 1;

            var angryBotTasksQueue = services.GetRequiredService<AngryBotTasksQueue>();
            var angryBotWorkers = Enumerable.Range(0, angryBotWorkersCount).Select(idx => RunInstance(idx, angryBotTasksQueue, token));
            workers.AddRange(angryBotWorkers);

            var commandBotTasksQueue = services.GetRequiredService<CommandBotTasksQueue>();
            var commandBotWorkers = Enumerable.Range(0, commandBotWorkersCount).Select(idx => RunInstance(idx, commandBotTasksQueue, token));
            workers.AddRange(commandBotWorkers);

            var urlBotTasksQueue = services.GetRequiredService<UrlBotTasksQueue>();
            var urlBotWorkers = Enumerable.Range(0, urlBotWorkersCount).Select(idx => RunInstance(idx, urlBotTasksQueue, token));
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
                    logger.LogInformation($"Worker {idx}: Processing task. Left tasks in queue {tasksQueue.GetName()}: {tasksQueue.Size}.");
                    await workItem(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
            }
        }
    }
}
