using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebChat.ApiRequestsObjects;
using WebChat.Services.Contract;

namespace WebChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BotsController: ControllerBase
    {
        private readonly IChatService chatService;
        public BotsController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpPost("AddToChat")]
        public async Task<IActionResult> AddBotToChat(ChatBot bot)
        {
            await chatService.AddBotToChat(bot.Name, bot.ChatId);
            return Ok();
        }

        [HttpPost("RemoveFromChat")]
        public async Task<IActionResult> RemoveBotFromChat(ChatBot bot)
        {
            await chatService.RemoveBotFromChat(bot.Name, bot.ChatId);
            return Ok();
        }
    }
}
