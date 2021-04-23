using System;

namespace WebChatData.Models.ModelInterfaces
{
    public interface IChatEvent
    {
        string EventKey { get; set; }
        string Description { get; set; }

        DateTime EventDate { get; set; } 
    }
}
