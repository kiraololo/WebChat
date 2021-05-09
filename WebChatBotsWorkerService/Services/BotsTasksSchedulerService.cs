using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebChatBotsWorkerService.BotsQueue.Contract;
using WebChatBotsWorkerService.Infrastructure;
using WebChatBotsWorkerService.Workers;

namespace WebChatBotsWorkerService.Services
{
    public class BotsTasksSchedulerService : IHostedService, IDisposable
    {
        private readonly IServiceProvider services;
        private readonly ILogger<BotsTasksSchedulerService> logger;
        private Timer timer;
        private readonly object syncRoot = new object();

        public BotsTasksSchedulerService(IServiceProvider services, ILogger<BotsTasksSchedulerService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = services.GetRequiredService<IConfiguration>();
            var runInterval = config.GetSection(BotsConstants.ConfigSections.BotsSettingsSection)
                .GetSection(BotsConstants.ConfigSections.RunIntervalSection).Value?.ToInt(1) ?? 1;
            timer = new Timer(
                (e) => ProcessBotsTasks(cancellationToken),
                null,
                TimeSpan.Zero, TimeSpan.FromSeconds(runInterval)
            );
            return Task.CompletedTask;
        }        

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        private void ProcessBotsTasks(CancellationToken token)
        {
            if (Monitor.TryEnter(syncRoot))
            {
                logger.LogInformation("Filling bots tasks queue");
                var bots = services.GetRequiredService<IEnumerable<IBot>>();
                foreach(var bot in bots)
                {
                    BotTasksSet(bot, token);
                }
                Monitor.Exit(syncRoot);
            }
            else
            {
                logger.LogInformation("Bots tasks queue filling skipped, because it in progress now");
            }
        }

        private void BotTasksSet(IBot bot, CancellationToken token)
        {
            var queue = services.GetRequiredService<IBotsTasksQueue>();
            queue.QueueBackgroundWorkItems(bot.GetTasks(token));
        }
    }
}
