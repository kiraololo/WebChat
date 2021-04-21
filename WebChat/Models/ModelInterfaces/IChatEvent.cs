using System;

namespace WebChat.Models.ModelInterfaces
{
    public interface IChatEvent
    {
        string EventKey { get; set; }
        string Description { get; set; }

        DateTime EventDate { get; set; } 
    }
}
