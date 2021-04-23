using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebChatBotsWorkerService;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChat.Inftastructure.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<AppDbContext>();
            if (!context.Chats.Any())
            {
                context.Chats.Add(new Chat
                {
                    Title = "World"
                });
                context.SaveChanges();
            }

            if (!context.ChatUsers.Any())
            {
                context.ChatUsers.Add(new ChatUser
                {
                    LoginName = "test@mail.ru",
                    NikName = "test_user_ololo"
                });
                context.SaveChanges();
            }

            if (!context.Bots.Any(b => b.Name == BotsConstants.Bots.AngryBot))
            {
                context.Bots.Add(new Bot
                {
                    Name = BotsConstants.Bots.AngryBot
                });
                context.SaveChanges();
            }

            if (!context.Bots.Any(b => b.Name == BotsConstants.Bots.CommandBot))
            {
                context.Bots.Add(new Bot
                {
                    Name = BotsConstants.Bots.CommandBot
                });
                context.SaveChanges();
            }

            if (!context.Bots.Any(b => b.Name == BotsConstants.Bots.UrlBot))
            {
                context.Bots.Add(new Bot
                {
                    Name = BotsConstants.Bots.UrlBot
                });
                context.SaveChanges();
            }

            if(!context.AngryBotDictionary.Any())
            {
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "привет", Answer = "Хрен тебе в ответ!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "здрасте", Answer = "Забор покрасте!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary
                {
                    KeyWord = "здравствуйте", Answer = "Здоровей видали и не боялись!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "меня зовут", Answer = "Говеное имя! Не пиши сюда больше!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "Андрей", Answer = "Андрей? Где-то я это уже слышал! Штрокую на широкую!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "моя мама", Answer = "Да я твою мамку в кино водил!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "мою маму", Answer = "Да я твою мамку в кино водил!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "я хочу", Answer = "Да мне плевать, что ты там хочешь!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "такая красивая", Answer = "Как кобыла сивая!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "доброе утро", Answer = "Утро добрым не бывает!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "пока", Answer = "Ты знаешь, что надо дома сделать!"
                });
                context.AngryBotDictionary.Add(new AngryBotDictionary {
                    KeyWord = "до свидания", Answer = "Наконец-то!"
                });
                context.SaveChanges();
            }

            if(!context.Synchronizations.Any())
            {
                context.Synchronizations.Add(new Synchronization
                {
                    SyncDate = DateTime.MinValue
                });
                context.SaveChanges();
            }
        }
    }
}
