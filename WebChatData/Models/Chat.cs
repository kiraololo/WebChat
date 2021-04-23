using System;
using System.Collections.Generic;
using System.Linq;

namespace WebChatData.Models
{
    public class Chat
    {
        public int ChatID { get; set; }

        public string Title { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public ChatUser Owner { get; set; }

        public ICollection<ChatUser> Members { get; set; }

        public ICollection<Message> History { get; set; }

        public ICollection<ChatUserEvent> ChatUserEvents { get; set; }
        public ICollection<ChatEvent> ChatEvents { get; set; }

        public int MembersCount => Members?.Count() ?? 0;

        public ICollection<Bot> Bots { get; set; }
    }
}
