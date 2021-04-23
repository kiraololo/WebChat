using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChatData.Models;

namespace WebChat.Services.Contract
{
    public interface IChatService
    {
        IQueryable<Chat> Chats { get; }

        void CreateChat(string title, int? ownerId = null);

        void RenameChat(int chatId, string chatTitle);

        void AddMember(int chatId, int memberId);

        void DeleteMember(int chatId, int memberId);

        void PutOwner(int chatId, int memberId);

        void DeleteChat(int id);

        IQueryable<Message> Messages { get; }

        IEnumerable<Message> GetMessages(int chatId, int userId, int take= 100, int skip = 0);

        void SendMessage(int chatId, string message);

        void ReadMessage(int messageId);

        void ChangeMessage(int id, string newMessage);

        Task DeleteMessage(int messageId, int userId);

        Task AddBotToChat(string name, int chatId);

        Task RemoveBotFromChat(string name, int chatId);
    }
}
