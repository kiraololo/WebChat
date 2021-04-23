namespace WebChat.ApiRequestsObjects
{
    public class SendingMessage
    {
        public int ChatId { get; set; }
        public string Message { get; set; }

        public int UserId { get; set; }
    }
}
