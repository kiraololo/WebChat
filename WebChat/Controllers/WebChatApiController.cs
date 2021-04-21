using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebChat.Inftastructure.Helpers;
using WebChat.Models;
using WebChat.Models.ApiObjects;
using WebChat.Models.Autorization;
using WebChat.Models.Repositories.Contract;
using WebChat.Services.Contract;

namespace WebChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WebChatApiController : ControllerBase
    {
        private IChatRepository repository;
        private IUserService userService;
        private IMapper mapper;
        private readonly SecretSettings secretSettings;

        public WebChatApiController(IChatRepository repo, IUserService usersService, IMapper mpr, IOptions<SecretSettings> scrtSettings)
        {
            repository = repo;
            userService = usersService;
            mapper = mpr;
            secretSettings = scrtSettings.Value;
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

            // return basic user info and authentication token
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
            // map model to entity
            var user = mapper.Map<ApplicationUser>(model);
            try
            {
                // create user
                userService.Create(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
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
        public IActionResult Update(int id, [FromBody] UpdateModel model)
        {
            // map model to entity and set id
            var user = mapper.Map<ApplicationUser>(model);
            user.Id = id;

            try
            {
                // update user 
                userService.Update(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Users/Delete")]
        public IActionResult Delete(int id)
        {
            userService.Delete(id);
            return Ok();
        }

        [HttpGet("Chats")]
        //[Route("Chats")]
       public IEnumerable<Chat> Get()
            => repository.Chats;        

        [HttpGet("Chats/GetById")]
        //[Route("Chats/GetById", Name = "id")]
        public Chat GetById(int id)
            => repository.Chats.FirstOrDefault(c => c.ChatID == id);

        [HttpGet("Chats/GetByTitle")]
        //[Route("Chats/GetByTitle", Name = "title")]
        public IEnumerable<Chat> GetByTitle(string title)
            => repository.Chats.Where(c=>c.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));

        [HttpPost("Chats/Create")]
        //[Route("Chats/Create")]
        public void CreateChat(ChatShort chat)
            => repository.CreateChat(chat.Title, chat.OwnerId);

        [HttpPut("Chats/Rename")]
        //[Route("Chats/Rename")]
        public void RenameChat(ChatNewTitle chat)
            => repository.RenameChat(chat.ChatId, chat.ChatTitle);

        [HttpPost("Chats/AddMember")]
        //[Route("Chats/AddMember")]        
        public void AddMember(ChatMember member)
            => repository.AddMember(member.ChatId, member.MemberId);

        [HttpDelete("Chats/DeleteMember")]
        //[Route("Chats/DeleteMember")]
        public void DeleteMember(int chatId, int memberId)
            => repository.DeleteMember(chatId, memberId);

        [HttpPut("Chats/PutOwner")]
        //[Route("Chats/PutOwner")]
        public void PutOwner(ChatMember owner)
            => repository.PutOwner(owner.ChatId, owner.MemberId);

        [HttpDelete("Chats/Delete")]
        //[Route("Chats/Delete")]
        public void DeleteChat(int id)
            => repository.DeleteChat(id);

        [HttpGet("Messages")]
        //[Route("Messages")]
        public IEnumerable<Message> GetMessages(int chatId, int take=100, int skip = 0)
            => repository.GetMessages(chatId, take, skip);

        [HttpPost("Messages/Send")]
        //[Route("Messages/Send")]
        public void SendMessage(SendingMessage message)
            => repository.SendMessage(message.ChatId, message.Message);

        [HttpPut("Messages/Read")]
        //[Route("Messages/Read")]
        public void ReadMessage([FromBody]int messageId)
            => repository.ReadMessage(messageId);

        [HttpPut("Messages/Change")]
        //[Route("Messages/Change")]
        public void ReadMessage(ChangedMessage message)
            => repository.ChangeMessage(message.MessageId, message.NewMessage);

        [HttpDelete("Messages/Delete")]
        //[Route("Messages/Delete")]
        public void DeleteMessage(int id)
            => repository.DeleteMessage(id);

        [HttpPost("Bots/AddToChat")]
        public void AddBotToChat(ChatBot bot)
            => repository.AddBotToChat(bot.Name, bot.ChatId);

        [HttpPost("Bots/RemoveFromChat")]
        public void RemoveBotFromChat(ChatBot bot)
            => repository.RemoveBotFromChat(bot.Name, bot.ChatId);
    }
}
