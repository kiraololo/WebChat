using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChatBotsWorkerService.Workers
{
    public class UrlBotWorker
    {
        private AppDbContext context;
        public UrlBotWorker(AppDbContext context)
        {
            this.context = context;
        }

        public async Task RunAsync(int chatId, string url, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
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
            catch(Exception ex)
            {
                await SendReportToChat(chat, "Произошла ошибка сохранения файла");
            }
        }

        private async Task SendReportToChat(Chat chat, string report)
        {
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
