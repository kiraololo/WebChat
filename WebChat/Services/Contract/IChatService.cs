using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChatData.Models;

namespace WebChat.Services.Contract
{
    public interface IChatService
    {
        IQueryable<Chat> Chats { get; }

        IQueryable<Message> Messages { get; }

        Task CreateChat(string title, int? ownerId = null);

        Task RenameChat(int chatId, string chatTitle);

        Task AddMember(int chatId, int memberId);

        Task DeleteMember(int chatId, int currentUserId, int deleteMemberId);

        Task PutOwner(int chatId, int memberId);

        Task DeleteChat(int id);        

        Task<IEnumerable<Message>> GetMessages(int chatId, int userId, int take= 100, int skip = 0);

        Task SendMessage(int chatId, int userId, string message);

        Task ReadMessage(int messageId);

        Task ChangeMessage(int id, string newMessage, int userId);

        Task DeleteMessage(int messageId, int userId);

        Task AddBotToChat(string name, int chatId);

        Task RemoveBotFromChat(string name, int chatId);
    }
}
