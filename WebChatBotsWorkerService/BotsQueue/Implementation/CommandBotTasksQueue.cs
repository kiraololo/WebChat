namespace WebChatBotsWorkerService.BotsQueue.Implementation
{
    public class CommandBotTasksQueue : BotTasksQueueBase{
        public new string GetName()
        {
            return nameof(CommandBotTasksQueue);
        }
    }
}
