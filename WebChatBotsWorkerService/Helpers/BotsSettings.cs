namespace WebChatBotsWorkerService.Helpers
{
    public class BotsSettings
    {
        public const string BotsSettingsSection = "BotsSettings";

        public int RunInterval { get; set; }
        public int WorkersCount { get; set; }
    }
}
