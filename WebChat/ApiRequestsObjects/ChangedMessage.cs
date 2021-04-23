namespace WebChat.ApiRequestsObjects
{
    public class ChangedMessage
    {
        public int MessageId { get; set; }
        public string NewMessage { get; set; }

        public int UserId { get; set; }
    }
}
