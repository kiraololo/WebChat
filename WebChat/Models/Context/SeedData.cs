using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WebChat.Inftastructure;

namespace WebChat.Models.Context
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<AppDbContext>();
            if (!context.Chats.Any())
            {
                context.Chats.Add(new Chat { 
                    Title= "World"
                });
                context.SaveChanges();
            }

            if(!context.ChatUsers.Any())
            {
                context.ChatUsers.Add(new ChatUser { 
                    LoginName = "test@mail.ru",
                    NikName = "test_user_ololo"
                });
                context.SaveChanges();
            }

            if(!context.Bots.Any(b=>b.Name == Constants.Bots.AngryBot))
            {
                context.Bots.Add(new Bot { 
                    Name = Constants.Bots.AngryBot
                });
                context.SaveChanges();
            }
        }
    }
}
