using System;
using System.Text.Json.Serialization;

namespace WebChatData.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        
        public ChatUser From { get; set; }
        public string MessageText { get; set; }

        public DateTime SentDate { get; set; }

        public bool IsReaded { get; set; }

        public bool IsEdited { get; set; }

        [JsonIgnore]
        public Chat Chat { get; set; }

        public bool FromBot { get; set; }

        public string BotName { get; set; }
    }
}
