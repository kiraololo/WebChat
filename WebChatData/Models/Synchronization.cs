using System;
using System.ComponentModel.DataAnnotations;

namespace WebChatData.Models
{
    public class Synchronization
    {
        [Key]
        public int SyncId { get; set; }

        public DateTime SyncDate { get; set; }
    }
}
