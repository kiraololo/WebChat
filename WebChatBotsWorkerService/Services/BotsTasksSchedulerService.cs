using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebChatBotsWorkerService.BotsQueue.Implementation;
using WebChatBotsWorkerService.Workers;
using WebChatDataData.Models.Context;

namespace WebChatBotsWorkerService.Services
{
    public class BotsTasksSchedulerService : IHostedService, IDisposable
    {
        private readonly IServiceProvider services;
        private Timer timer;
        private readonly object syncRoot = new object();

        public BotsTasksSchedulerService(IServiceProvider services)
        {
            this.services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(
                (e) => ProcessBotsTasks(),
                null,
                TimeSpan.Zero, TimeSpan.FromSeconds(1)
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
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var lastSync = context.Synchronizations.FirstOrDefault();
                var lastSyncDate = lastSync?.SyncDate ?? DateTime.MinValue;

                var newMessages = context.Messages.Include(m => m.Chat.Bots)
                    .Where(m => m.SentDate > lastSyncDate && m.Chat.Bots.Count > 0);
                foreach(var message in newMessages)
                {
                    var isCommand = message.MessageText?.StartsWith('\\') ?? false; 
                    var isUrl = (message.MessageText?.StartsWith("http") ?? false) 
                                    || (message.MessageText?.StartsWith("file:/") ?? false);
                    var isTxt = !isUrl && !isCommand;
                    foreach(var bot in message.Chat.Bots)
                    {
                        switch(bot.Name)
                        {
                            case BotsConstants.Bots.AngryBot:
                                if (isTxt)
                                {
                                    AngryBotTasksSet(message.Chat.ChatID, message.MessageText);
                                }
                                break;
                            case BotsConstants.Bots.CommandBot:
                                if (isCommand)
                                {
                                    CommandBotTasksSet(message.Chat.ChatID, message.MessageText);
                                }
                                break;
                            case BotsConstants.Bots.UrlBot:
                                if (isUrl)
                                {
                                    UrlBotTasksSet(message.Chat.ChatID, message.MessageText.Split(" ").First());
                                }
                                break;
                        }
                    }
                }
                if (lastSync != null)
                {
                    lastSync.SyncDate = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        private void AngryBotTasksSet(int chatId, string message)
        {
            var queue = services.GetRequiredService<AngryBotTasksQueue>();
            var worker = services.GetRequiredService<AngryBotWorker>();
            queue.QueueBackgroundWorkItem(token =>
            {
                return worker.RunAsync(chatId, message, token);
            });
        }

        private void CommandBotTasksSet(int chatId, string command)
        {
            var queue = services.GetRequiredService<CommandBotTasksQueue>();
            var worker = services.GetRequiredService<CommandBotWorker>();
            queue.QueueBackgroundWorkItem(token =>
            {
                return worker.RunAsync(chatId, command, token);
            });
        }

        private void UrlBotTasksSet(int chatId, string url)
        {
            var queue = services.GetRequiredService<UrlBotTasksQueue>();
            var worker = services.GetRequiredService<UrlBotWorker>();
            queue.QueueBackgroundWorkItem(token =>
            {
                return worker.RunAsync(chatId, url, token);
            });
        }
    }
}
