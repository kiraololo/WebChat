using System.ComponentModel.DataAnnotations;

namespace WebChatData.Models
{
    public class AngryBotDictionary
    {
        [Key]
        public int EntID { get; set; }
        public string KeyWord { get; set; }
        public string Answer { get; set; }
    }
}
