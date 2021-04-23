using System;
using System.ComponentModel.DataAnnotations;

namespace WebChatData.Models
{
    public class LeavedChat
    {
        [Key]
        public int ID { get; set; }
        public int ChatID { get; set; }
        public DateTime LeaveDate { get; set; }
    }
}
