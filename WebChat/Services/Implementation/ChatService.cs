using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChat.Inftastructure;
using WebChat.Services.Contract;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChat.Services.Implementation
{
    public class ChatService : IChatService
    {
        private AppDbContext context;

        public ChatService(AppDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Chat> Chats => context.Chats
            .Include(c => c.Members)
            .Include(c => c.History)
            .Include(c => c.Bots)
            .Include(c => c.Owner)
            .Include(c => c.ChatEvents).Include(c => c.ChatUserEvents);

        public IQueryable<Message> Messages
            => context.Messages;

        public async Task AddMember(int chatId, int memberId)
        {
            var member = await context.ChatUsers
                .FirstOrDefaultAsync(u => u.UserID == memberId);
            if (member == null)
                return;
            var chat = await context.Chats
                .Include(c => c.Members)
                .Include(c => c.ChatUserEvents)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
            if (chat == null)
                return;
            if (!chat.Members.Any(m => m.UserID == memberId))
            {
                chat.Members.Add(member);
                chat.ChatUserEvents.Add(new ChatUserEvent {
                    EventKey = Constants.ChatEvent.UserEvents.UserJoinedTheChat,
                    Description = $"{DateTime.Now}: Пользователь {((member.NikName?.IsEmpty() ?? true) ? member.LoginName : member.NikName)} присоединился к чату",
                    User = member
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteMember(int chatId, int currentUserId, int deleteMemberId)
        {
            var memberIsCurrentUser = currentUserId == deleteMemberId;
            var chat = await context.Chats
                .Include(c => c.Members)
                .Include(c => c.ChatUserEvents)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
            if (chat == null)
                return;
            var member = chat.Members.FirstOrDefault(u => u.UserID == deleteMemberId);
            if (member == null)
                return;
            chat.Members.Remove(member);
            if (memberIsCurrentUser)
            {
                chat.ChatUserEvents.Add(new ChatUserEvent
                {
                    EventKey = Constants.ChatEvent.UserEvents.UserLeaveTheChat,
                    Description = $"{DateTime.Now}: Пользователь {((member.NikName?.IsEmpty() ?? true) ? member.LoginName : member.NikName)} покинул чат",
                    User = member
                });
            }
            else
            {
                chat.ChatUserEvents.Add(new ChatUserEvent
                {
                    EventKey = Constants.ChatEvent.UserEvents.UserWasRemovedFromChat,
                    Description = $"{DateTime.Now}: Пользователь {((member.NikName?.IsEmpty() ?? true) ? member.LoginName : member.NikName)} был удален из чата",
                    User = member
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task CreateChat(string title, int? ownerId = null)
        {
            var owner = ownerId.HasValue
                ? await context.ChatUsers.FirstOrDefaultAsync(u => u.UserID == ownerId.Value)
                : null;
            context.Chats.Add(new Chat {
                Title = title,
                Owner = owner
            });
            await context.SaveChangesAsync();
        }

        public async Task RenameChat(int chatId, string chatTitle)
        {
            if (chatTitle.IsEmpty())
                return;
            var chat = await context.Chats
                .Include(c => c.ChatEvents)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
            if (chat != null && chat.Title != chatTitle)
            {
                var oldTitle = chat.Title;
                chat.Title = chatTitle;
                chat.ChatEvents.Add(new ChatEvent {
                    EventKey = Constants.ChatEvent.ChatWasRenamed,
                    Description = $"{DateTime.Now}: Чат изменил название с \"{ oldTitle }\" на \"{ chatTitle }\""
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteChat(int id)
        {
            var chatToRemove = await context.Chats.FirstOrDefaultAsync(c => c.ChatID == id);
            if (chatToRemove != null)
            {
                context.Chats.Remove(chatToRemove);
                await context.SaveChangesAsync();
            }
        }

        public async Task PutOwner(int chatId, int memberId)
        {
            var owner = await context.ChatUsers
                .FirstOrDefaultAsync(u => u.UserID == memberId);
            if (owner == null)
                return;
            var chat = await context.Chats
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
            if (chat == null)
                return;
            chat.Owner = owner;
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetMessages(int chatId, int userId, int take = 100, int skip = 0)
        {
            var res = new List<Message>();
            var chat = await context.Chats
                .Include(c => c.History)
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c=>c.ChatID == chatId);
            if(chat != null)
            {
                if(chat.Members.Any(m => m.UserID == userId))
                {
                    res = chat.History.OrderBy(m => m.MessageID).ToList();
                }
                else
                {
                    var user = await context.ChatUsers.Include(u => u.LeavedChats)
                        .FirstOrDefaultAsync(u=>u.UserID == userId);
                    var leavedDate = user.LeavedChats.FirstOrDefault(lc => lc.ChatID == chatId)?.LeaveDate;
                    if(leavedDate.HasValue)
                    {
                        res = chat.History.Where(m 
                            => m.SentDate < leavedDate.Value)
                            .OrderBy(m=>m.MessageID).ToList();
                    }
                }
            }
            return res;
        }

        public async Task SendMessage(int chatId, int userId, string message)
        {
            var user = await context.ChatUsers.FirstOrDefaultAsync(u=>u.UserID == userId);
            if (user == null)
                return;
            var chat = await context.Chats.Include(c=>c.History)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
            if(chat != null)
            {
                chat.History.Add(new Message
                {
                    From = user,
                    MessageText = message,
                    SentDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task ReadMessage(int messageId)
        {
            var message = await context.Messages
                .FirstOrDefaultAsync(m=> m.MessageID == messageId);
            if(message != null && !message.IsReaded)
            {
                message.IsReaded = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task ChangeMessage(int messageId, string newMessage, int userId)
        {
            var user = await context.ChatUsers.FirstOrDefaultAsync(u=>u.UserID == userId);
            if (user == null)
                return;
            var message = await context.Messages.Include(m=>m.Chat)
                .FirstOrDefaultAsync(m => m.MessageID == messageId);
            if (message.MessageText == newMessage)
                return;
            var canChange = message.Chat?.Owner?.UserID == user.UserID;
            if (!canChange)
            {
                canChange = user.UserID == message.From.UserID
                                && message.SentDate.AddDays(1) > DateTime.Now;
            }
            if(canChange)
            {
                message.MessageText = newMessage;
                message.IsEdited = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteMessage(int messageId, int userId)
        {
            var user = await context.ChatUsers.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
                return;
            var message = await context.Messages.Include(m => m.Chat)
                .FirstOrDefaultAsync(m => m.MessageID == messageId);
            var canDelete = message.Chat?.Owner?.UserID == user.UserID;
            if (!canDelete)
            {
                canDelete = user.UserID == message.From.UserID
                                && message.SentDate.AddDays(1) > DateTime.Now;
            }
            if (canDelete)
            {
                context.Messages.Remove(message);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddBotToChat(string name, int chatId)
        {
            var bot = context.Bots.FirstOrDefault(b=>b.Name == name);
            if(bot != null)
            {
                var chat = context.Chats.Include(c=>c.Bots)
                                .FirstOrDefault(c=>c.ChatID == chatId);
                if(chat != null)
                {
                    chat.Bots.Add(bot);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveBotFromChat(string name, int chatId)
        {
            var bot = context.Bots.FirstOrDefault(b => b.Name == name);
            if (bot != null)
            {
                var chat = context.Chats.Include(c => c.Bots)
                                .FirstOrDefault(c => c.ChatID == chatId);
                if (chat != null)
                {
                    chat.Bots.Remove(bot);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
