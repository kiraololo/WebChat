using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebChatBotsWorkerService.BotsQueue.Contract;
using WebChatBotsWorkerService.Infrastructure;
using WebChatBotsWorkerService.Workers;
using WebChatData.Models;
using WebChatDataData.Models.Context;

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
                (e) => ProcessBotsTasks(),
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

        private void ProcessBotsTasks()
        {
            if (Monitor.TryEnter(syncRoot))
            {
                logger.LogInformation("Bots tasks queue filling started");
                using (var scope = services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var lastSync = context.Synchronizations.FirstOrDefault();
                    var lastSyncDate = lastSync?.SyncDate ?? DateTime.MinValue;

                    var newMessages = context.Messages.Include(m => m.Chat.Bots)
                        .Where(m => m.SentDate > lastSyncDate && m.Chat.Bots.Count > 0 && !m.FromBot);
                    foreach (var message in newMessages)
                    {
                        BotTasksSet(message);
                    }
                    if (lastSync != null)
                    {
                        lastSync.SyncDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                logger.LogInformation("Bots tasks queue filling finished");
                Monitor.Exit(syncRoot);
            }
            else
            {
                logger.LogInformation("Bots tasks queue filling skipped, because it in progress now");
            }
        }

        private void BotTasksSet(Message message)
        {
            var queue = services.GetRequiredService<IBotsTasksQueue>();
            var worker = services.GetRequiredService<IMessageBot>();
            queue.QueueBackgroundWorkItem(token =>
            {
                return worker.RunAsync(message, token);
            });
        }
    }
}
