using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChat.ApiRequestsObjects;
using WebChat.Services.Contract;
using WebChatData.Models;

namespace WebChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private IChatService chatService;

        public ChatsController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet]
        public IEnumerable<Chat> Get()
            => chatService.Chats;        

        [HttpGet("GetById")]
        public async Task<Chat> GetById(int id)
            => await chatService.Chats.FirstOrDefaultAsync(c => c.ChatID == id);

        [HttpGet("GetByTitle")]
        public IEnumerable<Chat> GetByTitle(string title)
            => chatService.Chats.Where(c=>c.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));

        [HttpPost("Create")]
        public async Task<IActionResult> CreateChat(ChatShort chat)
        {
            await chatService.CreateChat(chat.Title, chat.OwnerId);
            return Ok();
        }

        [HttpPut("Rename")]
        public async Task<IActionResult> RenameChat(ChatNewTitle chat)
        {
            await chatService.RenameChat(chat.ChatId, chat.ChatTitle);
            return Ok();
        }

        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMember(ChatMember member)
        {
            await chatService.AddMember(member.ChatId, member.MemberId);
            return Ok();
        }

        [HttpDelete("DeleteMember")]
        public async Task<IActionResult> DeleteMember(int chatId, int currentUserId, int deleteMemberId)
        {
            await chatService.DeleteMember(chatId, currentUserId, deleteMemberId);
            return Ok();
        }

        [HttpPut("PutOwner")]
        public async Task<IActionResult> PutOwner(ChatMember owner)
        {
            await chatService.PutOwner(owner.ChatId, owner.MemberId);
            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            await chatService.DeleteChat(id);
            return Ok();
        }
    }
}
