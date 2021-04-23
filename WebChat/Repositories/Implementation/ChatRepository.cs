using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChat.Inftastructure;
//using WebChat.Models.Context;
using WebChat.Repositories.Contract;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChat.Repositories.Implementation
{
    public class ChatRepository : IChatRepository
    {
        private AppDbContext context;

        public ChatRepository(AppDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Chat> Chats => context.Chats
            .Include(c => c.Members)
            .Include(c => c.History)
            .Include(c => c.Bots)
            .Include(c => c.Owner)
            .Include(c => c.ChatEvents).Include(c => c.ChatUserEvents);

        public void AddMember(int chatId, int memberId)
        {
            var member = context.ChatUsers.FirstOrDefault(u => u.UserID == memberId);
            if (member == null)
                return;
            var chat = context.Chats
                .Include(c => c.Members)
                .Include(c => c.ChatUserEvents)
                .FirstOrDefault(c => c.ChatID == chatId);
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
                context.SaveChanges();
            }
        }

        public void DeleteMember(int chatId, int memberId)
        {
            //TODO: проверка на самого себя
            var memberIsCurrentUser = true;

            var chat = context.Chats
                .Include(c => c.Members)
                .Include(c => c.ChatUserEvents)
                .FirstOrDefault(c => c.ChatID == chatId);
            if (chat == null)
                return;
            var member = chat.Members.FirstOrDefault(u => u.UserID == memberId);
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
            context.SaveChanges();
        }

        public void CreateChat(string title, int? ownerId = null)
        {
            var owner = ownerId.HasValue
                ? context.ChatUsers.FirstOrDefault(u => u.UserID == ownerId.Value)
                : null;
            context.Chats.Add(new Chat {
                Title = title,
                Owner = owner
            });
            context.SaveChanges();
            //context.SaveChangesAsync();
        }

        public void RenameChat(int chatId, string chatTitle)
        {
            if (chatTitle.IsEmpty())
                return;
            var chat = context.Chats
                .Include(c => c.ChatEvents)
                .FirstOrDefault(c => c.ChatID == chatId);
            if (chat != null && chat.Title != chatTitle)
            {
                var oldTitle = chat.Title;
                chat.Title = chatTitle;
                chat.ChatEvents.Add(new ChatEvent {
                    EventKey = Constants.ChatEvent.ChatWasRenamed,
                    Description = $"{DateTime.Now}: Чат изменил название с \"{ oldTitle }\" на \"{ chatTitle }\""
                });
                context.SaveChanges();
            }
        }

        public void DeleteChat(int id)
        {
            var chatToRemove = context.Chats.FirstOrDefault(c => c.ChatID == id);
            if (chatToRemove != null)
            {
                context.Chats.Remove(chatToRemove);
                context.SaveChanges();
            }
        }

        public void PutOwner(int chatId, int memberId)
        {
            var owner = context.ChatUsers.FirstOrDefault(u => u.UserID == memberId);
            if (owner == null)
                return;
            var chat = context.Chats
                .FirstOrDefault(c => c.ChatID == chatId);
            if (chat == null)
                return;
            chat.Owner = owner;
            context.SaveChanges();
            //context.SaveChangesAsync();
        }

        public IQueryable<Message> Messages
            => context.Messages;

        public IEnumerable<Message> GetMessages(int chatId, int userId, int take = 100, int skip = 0)
        {
            var res = new List<Message>();
            var chat = context.Chats
                .Include(c => c.History)
                .Include(c => c.Members)
                .FirstOrDefault(c=>c.ChatID == chatId);
            if(chat != null)
            {
                if(chat.Members.Any(m => m.UserID == userId))
                {
                    res = chat.History.OrderBy(m => m.MessageID).ToList();
                }
                else
                {
                    var user = context.ChatUsers.Include(u => u.LeavedChats)
                        .FirstOrDefault(u=>u.UserID == userId);
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

        public void SendMessage(int chatId, string message)
        {
            var USER_TEST_POTOM_UDALIT = context.ChatUsers.FirstOrDefault();

            var chat = context.Chats.Include(c=>c.History)
                .FirstOrDefault(c => c.ChatID == chatId);
            if(chat != null)
            {
                chat.History.Add(new Message
                {
                    From = USER_TEST_POTOM_UDALIT,
                    MessageText = message,
                    SentDate = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public void ReadMessage(int messageId)
        {
            var message = context.Messages.FirstOrDefault(m=> m.MessageID == messageId);
            if(message != null && !message.IsReaded)
            {
                message.IsReaded = true;
                context.SaveChanges();
            }
        }

        public void ChangeMessage(int id, string newMessage)
        {
            var CURRENT_USER_POTOM_PEREDELAT = context.ChatUsers.FirstOrDefault();
            
            var message = context.Messages.Include(m=>m.Chat)
                .FirstOrDefault(m => m.MessageID == id);
            if (message.MessageText == newMessage)
                return;
            var canChange = message.Chat?.Owner?.UserID == CURRENT_USER_POTOM_PEREDELAT.UserID;
            if (!canChange)
            {
                canChange = CURRENT_USER_POTOM_PEREDELAT.UserID == message.From.UserID
                                && message.SentDate.AddDays(1) > DateTime.Now;
            }
            if(canChange)
            {
                message.MessageText = newMessage;
                message.IsEdited = true;
                context.SaveChanges();
            }
        }

        public void DeleteMessage(int id)
        {
            var CURRENT_USER_POTOM_PEREDELAT = context.ChatUsers.FirstOrDefault();

            var message = context.Messages.Include(m => m.Chat)
                .FirstOrDefault(m => m.MessageID == id);
            var canDelete = message.Chat?.Owner?.UserID == CURRENT_USER_POTOM_PEREDELAT.UserID;
            if (!canDelete)
            {
                canDelete = CURRENT_USER_POTOM_PEREDELAT.UserID == message.From.UserID
                                && message.SentDate.AddDays(1) > DateTime.Now;
            }
            if (canDelete)
            {
                context.Messages.Remove(message);
                context.SaveChanges();
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


                //var bot = context.Bots.FirstOrDefault(b => b.Name == name);
                //if (bot != null)
                //{
                //    var chat = context.Chats.Include(c => c.Bots)
                //                    .FirstOrDefault(c => c.ChatID == chatId);
                //    if (chat != null)
                //    {
                //        chat.Bots.Remove(bot);
                //        context.SaveChanges();
                //    }
                //}
        }
    }
}
