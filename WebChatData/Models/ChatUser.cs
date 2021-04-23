using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebChatData.Models
{
    public class ChatUser
    {
        [Key]
        public int UserID { get; set; }

        public string NikName { get; set; }

        public string LoginName { get; set; }

        [JsonIgnore]
        public ICollection<Chat> Chats { get; set; }
    }
}
