using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebChatData.Models
{
    public class Bot
    {
        //[Key]
        public int BotId { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Chat> Chats { get; set; }
    }
}
