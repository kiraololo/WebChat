using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebChat.ApiRequestsObjects;
using WebChat.Services.Contract;
using WebChatData.Models;

namespace WebChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MessagesController: ControllerBase
    {
        private IChatService chatService;

        public MessagesController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet]
        public async Task<IEnumerable<Message>> GetMessages(int chatId, int userId, int take = 100, int skip = 0)
            => await chatService.GetMessages(chatId, userId, take, skip);


        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage(SendingMessage message)
        {
            await chatService.SendMessage(message.ChatId, message.UserId, message.Message);
            return Ok();
        }

        [HttpPut("Read")]
        public async Task<IActionResult> ReadMessage([FromBody] int messageId)
        {
            await chatService.ReadMessage(messageId);
            return Ok();
        }

        [HttpPut("Change")]
        public async Task<IActionResult> ChangeMessage(ChangedMessage message)
        {
            await chatService.ChangeMessage(message.MessageId, message.NewMessage, message.UserId);
            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteMessage(int messageId, int userId)
        {
            await chatService.DeleteMessage(messageId, userId);
            return Ok();
        }
    }
}
