using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebChatBotsWorkerService.BotsQueue.Contract;
using WebChatBotsWorkerService.Helpers;

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
            var botsConfiguration = services.GetRequiredService<IOptions<BotsSettings>>()?.Value;
            var botsTasksQueue = services.GetRequiredService<IBotsTasksQueue>();
            var workers = Enumerable.Range(0, botsConfiguration?.WorkersCount ?? 1).Select(idx
                                        => RunInstance(idx, botsTasksQueue, token));
            await Task.WhenAll(workers);
        }

        private async Task RunInstance(int idx, IBotsTasksQueue tasksQueue, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var workItem = await tasksQueue.DequeueAsync(token);
                try
                {
                    logger.LogInformation($"Worker {idx}: Processing task. Left tasks in queue: {tasksQueue.Size}.");
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
