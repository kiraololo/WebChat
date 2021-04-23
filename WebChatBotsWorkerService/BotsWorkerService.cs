using Microsoft.Extensions.Configuration;
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
using WebChatBotsWorkerService.Infrastructure;

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

            var config = services.GetRequiredService<IConfiguration>();
            var botsConfigSection = config.GetSection(BotsConstants.ConfigSections.BotsSettingsSection);

            var angryBotWorkersCount = botsConfigSection.GetSection(BotsConstants.Bots.AngryBot)
                .GetSection(BotsConstants.ConfigSections.WorkersCountSection).Value?.ToInt(2) ?? 2;
            var commandBotWorkersCount = botsConfigSection.GetSection(BotsConstants.Bots.CommandBot)
                .GetSection(BotsConstants.ConfigSections.WorkersCountSection).Value?.ToInt(2) ?? 2;
            var urlBotWorkersCount = botsConfigSection.GetSection(BotsConstants.Bots.UrlBot)
                .GetSection(BotsConstants.ConfigSections.WorkersCountSection).Value?.ToInt(2) ?? 2;

            var angryBotTasksQueue = services.GetRequiredService<AngryBotTasksQueue>();
            var angryBotWorkers = Enumerable.Range(0, angryBotWorkersCount).Select(idx 
                                        => RunInstance(idx, nameof(AngryBotTasksQueue), angryBotTasksQueue, token));
            workers.AddRange(angryBotWorkers);

            var commandBotTasksQueue = services.GetRequiredService<CommandBotTasksQueue>();
            var commandBotWorkers = Enumerable.Range(0, commandBotWorkersCount).Select(idx 
                                        => RunInstance(idx, nameof(CommandBotTasksQueue), commandBotTasksQueue, token));
            workers.AddRange(commandBotWorkers);

            var urlBotTasksQueue = services.GetRequiredService<UrlBotTasksQueue>();
            var urlBotWorkers = Enumerable.Range(0, urlBotWorkersCount).Select(idx 
                                        => RunInstance(idx, nameof(UrlBotTasksQueue), urlBotTasksQueue, token));
            workers.AddRange(urlBotWorkers);

            await Task.WhenAll(workers);
        }

        private async Task RunInstance(int idx, string queueName, IBotsTasksQueue tasksQueue, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var workItem = await tasksQueue.DequeueAsync(token);
                try
                {
                    logger.LogInformation($"Worker {idx}: Processing task. Left tasks in queue {queueName}: {tasksQueue.Size}.");
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
