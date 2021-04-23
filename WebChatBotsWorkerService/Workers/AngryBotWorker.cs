using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChatBotsWorkerService.Workers
{
    public class AngryBotWorker
    {
        private AppDbContext context;
        public AngryBotWorker(AppDbContext context)
        {
            this.context = context;
        }

        public async Task RunAsync(int chatId, string message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var chat = await context.Chats.Include(c => c.History)
                .FirstOrDefaultAsync(c=>c.ChatID == chatId);
            if(chat != null)
            {
                var answer = null as string;
                foreach(var word in message.Split(" "))
                {
                    answer = context.AngryBotDictionary
                        .FirstOrDefault(d => d.KeyWord.ToLower() == word.ToLower())?.Answer;
                    if (answer != null)
                        break;
                }
                if(!string.IsNullOrEmpty(answer))
                {
                    chat.History.Add(new Message { 
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
}
