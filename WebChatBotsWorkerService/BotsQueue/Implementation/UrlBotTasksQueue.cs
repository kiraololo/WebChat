namespace WebChatBotsWorkerService.BotsQueue.Implementation
{
    public class UrlBotTasksQueue : BotTasksQueueBase {
        public new string GetName()
        {
            return nameof(UrlBotTasksQueue);
        }
    }
}
