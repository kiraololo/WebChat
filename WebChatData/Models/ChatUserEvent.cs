using System;
using System.ComponentModel.DataAnnotations;
using WebChatData.Models.ModelInterfaces;

namespace WebChatData.Models
{
    public class ChatUserEvent : IChatEvent
    {
        [Key]
        public int EventID { get; set; }

        public string EventKey { get; set; }

        public string Description { get; set; }

        public ChatUser User { get; set; }

        public DateTime EventDate { get; set; } = DateTime.Now;
    }
}
