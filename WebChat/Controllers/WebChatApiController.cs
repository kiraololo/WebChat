using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebChat.ApiRequestsObjects;
using WebChat.Inftastructure.Helpers;
using WebChat.Services.Contract;
using WebChatData.Models;
using WebChatData.Models.Autorization;
using WebChatDataData.Models.Autorization;

namespace WebChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WebChatApiController : ControllerBase
    {
        private IChatService chatService;
        private IUserService userService;
        private IMapper mapper;
        private readonly SecretSettings secretSettings;

        public WebChatApiController(IChatService chatService, IUserService userService,
            IMapper mapper, IOptions<SecretSettings> secretSettings)
        {
            this.chatService = chatService;
            this.userService = userService;
            this.mapper = mapper;
            this.secretSettings = secretSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("Auth/Authenticate")]
        public IActionResult Authenticate(AuthenticateModel model)
        {
            var user = userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("Auth/Register")]
        public IActionResult Register(RegisterModel model)
        {
            var user = mapper.Map<ApplicationUser>(model);
            try
            {
                userService.Create(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Users/GetAll")]
        public IActionResult GetAllUsers()
        {
            var users = userService.GetAll();
            var model = mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        [HttpGet("Users/GetById")]
        public IActionResult GetUserById(int id)
        {
            var user = userService.GetById(id);
            var model = mapper.Map<UserModel>(user);
            return Ok(model);
        }

        [HttpPut("Users/Update")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModel model)
        {
            var user = mapper.Map<ApplicationUser>(model);
            user.Id = id;
            try
            {
                await userService.Update(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Users/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await userService.Delete(id);
            return Ok();
        }

        [HttpGet("Chats")]
       public IEnumerable<Chat> Get()
            => chatService.Chats;        

        [HttpGet("Chats/GetById")]
        public async Task<Chat> GetById(int id)
            => await chatService.Chats.FirstOrDefaultAsync(c => c.ChatID == id);

        [HttpGet("Chats/GetByTitle")]
        public IEnumerable<Chat> GetByTitle(string title)
            => chatService.Chats.Where(c=>c.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));

        [HttpPost("Chats/Create")]
        public async Task<IActionResult> CreateChat(ChatShort chat)
        {
            await chatService.CreateChat(chat.Title, chat.OwnerId);
            return Ok();
        }

        [HttpPut("Chats/Rename")]
        public async Task<IActionResult> RenameChat(ChatNewTitle chat)
        {
            await chatService.RenameChat(chat.ChatId, chat.ChatTitle);
            return Ok();
        }

        [HttpPost("Chats/AddMember")]
        public async Task<IActionResult> AddMember(ChatMember member)
        {
            await chatService.AddMember(member.ChatId, member.MemberId);
            return Ok();
        }

        [HttpDelete("Chats/DeleteMember")]
        public async Task<IActionResult> DeleteMember(int chatId, int currentUserId, int deleteMemberId)
        {
            await chatService.DeleteMember(chatId, currentUserId, deleteMemberId);
            return Ok();
        }

        [HttpPut("Chats/PutOwner")]
        public async Task<IActionResult> PutOwner(ChatMember owner)
        {
            await chatService.PutOwner(owner.ChatId, owner.MemberId);
            return Ok();
        }

        [HttpDelete("Chats/Delete")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            await chatService.DeleteChat(id);
            return Ok();
        }            

        [HttpGet("Messages")]
        public async Task<IEnumerable<Message>> GetMessages(int chatId, int userId, int take = 100, int skip = 0)
            => await chatService.GetMessages(chatId, userId, take, skip);
        

        [HttpPost("Messages/Send")]
        public async Task<IActionResult> SendMessage(SendingMessage message)
        {
            await chatService.SendMessage(message.ChatId, message.UserId, message.Message);
            return Ok();
        }

        [HttpPut("Messages/Read")]
        public async Task<IActionResult> ReadMessage([FromBody]int messageId)
        {
            await chatService.ReadMessage(messageId);
            return Ok();
        }            

        [HttpPut("Messages/Change")]
        public async Task<IActionResult> ChangeMessage(ChangedMessage message)
        {
            await chatService.ChangeMessage(message.MessageId, message.NewMessage, message.UserId);
            return Ok();
        }

        [HttpDelete("Messages/Delete")]
        public async Task<IActionResult> DeleteMessage(int messageId, int userId)
        {
            await chatService.DeleteMessage(messageId, userId);
            return Ok();
        }   

        [HttpPost("Bots/AddToChat")]
        public async Task<IActionResult> AddBotToChat(ChatBot bot)
        {
            await chatService.AddBotToChat(bot.Name, bot.ChatId);
            return Ok();
        }

        [HttpPost("Bots/RemoveFromChat")]
        public async Task<IActionResult> RemoveBotFromChat(ChatBot bot)
        {
            await chatService.RemoveBotFromChat(bot.Name, bot.ChatId);
            return Ok();
        }
    }
}
