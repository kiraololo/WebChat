using System.ComponentModel.DataAnnotations;

namespace WebChatDataData.Models.Autorization
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
