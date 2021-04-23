using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChatBotsWorkerService.Workers
{
    public class CommandBotWorker
    {
        private AppDbContext context;

        public CommandBotWorker(AppDbContext context)
        {
            this.context = context;
        }

        public async Task RunAsync(int chatId, string message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var chat = await context.Chats.Include(c => c.History).Include(c=>c.Members)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
            if (chat != null)
            {
                var botMessage = "НЕИЗВЕСТНАЯ КОМАНДА!";
                switch(message)
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
}
