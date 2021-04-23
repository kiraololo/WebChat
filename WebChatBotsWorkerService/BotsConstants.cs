namespace WebChatBotsWorkerService
{
    public static class BotsConstants
    {
        public static class Bots
        {
            public const string AngryBot = "AngryBot";
            public const string CommandBot = "CommandBot";
            public const string UrlBot = "UrlBot";
        }

        public static class CommandBot
        {
            public static class Commands
            {
                public const string Help = "\\help";
                public const string UsersCount = "\\users_count";
                public const string GetOwner = "\\get_owner";
                public const string ChatAge = "\\chat_age";
            }

            public static class DefaultMessages
            {
                public const string HelpMessage = "Список команд: \\help- помощь; \\users_count- получить кол-во пользователей чата; \\get_owner- получить имя владельца чата; \\chat_age- получить возраст чата";
            }
        }

        public static class ConfigSections
        {
            public const string BotsSettingsSection = "BotsSettings";
            public const string WorkersCountSection = "WorkersCount";
            public const string RunIntervalSection = "RunInterval";
        }
    }
}
