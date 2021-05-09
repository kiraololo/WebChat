using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChatBotsWorkerService.Workers
{
    public class MessageBot : IBot
    {
        private readonly IServiceProvider services;
        public MessageBot(IServiceProvider services)
        {
            this.services = services;
        }

        public string BotName => "MessageBot";

        public  IEnumerable<Func<CancellationToken, Task>> GetTasks(CancellationToken token)
            => GetNewMessages()?.Select<Message, Func<CancellationToken, Task>>(m => (token) => ProcessTask(m, token));

        private async Task ProcessTask(Message message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!message?.Chat?.Bots.Any() ?? false
               || string.IsNullOrEmpty(message?.MessageText))
                return;

            var messageText = message.MessageText;
            var isCommand = messageText.StartsWith('\\');
            var isUrl = messageText.StartsWith("http")
                            || messageText.StartsWith("file:/");
            var isTxt = !isUrl && !isCommand;

            if (isTxt && message.Chat.Bots.Any(cb => cb.Name == BotsConstants.Bots.AngryBot))
            {
                await SayAngry(message.Chat.ChatID, messageText);
            }
            if (isCommand && message.Chat.Bots.Any(cb => cb.Name == BotsConstants.Bots.CommandBot))
            {
                await ProcessCommand(message.Chat.ChatID, messageText);
            }
            if (isUrl && message.Chat.Bots.Any(cb => cb.Name == BotsConstants.Bots.UrlBot))
            {
                await DownloadFile(message.Chat.ChatID, messageText.Split(" ").First());
            }
        } 
        
        private IEnumerable<Message> GetNewMessages()
        {
            IEnumerable<Message> newMessages = null;
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var lastSync = context.Synchronizations.FirstOrDefault();
                var lastSyncDate = lastSync?.SyncDate ?? DateTime.MinValue;
                newMessages = context.Messages.Include(m => m.Chat.Bots)
                    .Where(m => m.SentDate > lastSyncDate && m.Chat.Bots.Count > 0 && !m.FromBot).ToList();
                if (lastSync != null)
                {
                    lastSync.SyncDate = DateTime.Now;
                    context.SaveChanges();
                }                
            }
            return newMessages;
        }

        private async Task SayAngry(int chatId, string message)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var chat = await context.Chats.Include(c => c.History)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
                if (chat != null)
                {
                    var answer = null as string;
                    foreach (var word in message.Split(" "))
                    {
                        answer = context.AngryBotDictionary
                            .FirstOrDefault(d => d.KeyWord.ToLower() == word.ToLower())?.Answer;
                        if (answer != null)
                            break;
                    }
                    if (!string.IsNullOrEmpty(answer))
                    {
                        chat.History.Add(new Message
                        {
                            BotName = BotsConstants.Bots.AngryBot,
                            FromBot = true,
                            MessageText = answer,
                            SentDate = DateTime.Now
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task ProcessCommand(int chatId, string command)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var chat = await context.Chats.Include(c => c.History).Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
                if (chat != null)
                {
                    var botMessage = "НЕИЗВЕСТНАЯ КОМАНДА!";
                    switch (command)
                    {
                        case BotsConstants.CommandBot.Commands.Help:
                            botMessage = BotsConstants.CommandBot.DefaultMessages.HelpMessage;
                            break;
                        case BotsConstants.CommandBot.Commands.UsersCount:
                            botMessage = $"Количество участников: {chat.MembersCount}";
                            break;
                        case BotsConstants.CommandBot.Commands.GetOwner:
                            botMessage = $"Владелец чата: {chat.Owner?.LoginName}";
                            break;
                        case BotsConstants.CommandBot.Commands.ChatAge:
                            botMessage = $"Возраст чата: {(DateTime.Now - chat.Created).TotalDays} дней";
                            break;
                    }
                    chat.History.Add(new Message
                    {
                        BotName = BotsConstants.Bots.CommandBot,
                        FromBot = true,
                        MessageText = botMessage,
                        SentDate = DateTime.Now
                    });
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task DownloadFile(int chatId, string url)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var chat = await context.Chats.Include(c => c.History)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
                var name = System.IO.Path.GetFileNameWithoutExtension(url.Split('?')[0]);
                var extension = System.IO.Path.GetExtension(url.Split('?')[0]);
                try
                {
                    using (var wc = new WebClient())
                    {
                        wc.DownloadFile(new Uri(url),
                            $"C:\\ChatFiles\\{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}_file_{name.Replace(" ", "_")}{extension}");
                    }
                    await SendReportToChat(chat, "Файл успешно сохранен");
                }
                catch (Exception ex)
                {
                    await SendReportToChat(chat, $"Произошла ошибка сохранения файла: {ex.Message}");
                }
            }
        }

        private async Task SendReportToChat(Chat chat, string report)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (chat != null)
                {
                    chat.History.Add(new Message
                    {
                        BotName = BotsConstants.Bots.UrlBot,
                        FromBot = true,
                        MessageText = report,
                        SentDate = DateTime.Now
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
