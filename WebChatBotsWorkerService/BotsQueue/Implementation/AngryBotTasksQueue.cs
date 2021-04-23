namespace WebChatBotsWorkerService.BotsQueue.Implementation
{
    public class AngryBotTasksQueue: BotTasksQueueBase{
        public new string GetName()
        {
            return nameof(AngryBotTasksQueue);
        }
    }
}
