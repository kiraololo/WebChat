namespace WebChat.Inftastructure
{
    public static class Constants
    {
        public class Settings
        {
            public class ConfigSections
            {
                public const string SecretSettingsSection = "SecretSettings";
                public const string DataSection = "Data";
                public const string ChatWebSection = "ChatWeb";
                public const string ChatIdentitySection = "ChatIdentity";
                public const string ConStringSection = "ConnectionString";
            }
        }

        public static class ChatEvent
        {
            public static class UserEvents
            {
                public const string UserJoinedTheChat = "UserJoinedTheChat";
                
                public const string UserLeaveTheChat = "UserLeaveTheChat";
                
                public const string UserWasRemovedFromChat = "UserWasRemovedFromChat";
            }

            public const string ChatWasRenamed = "ChatWasRenamed";
        }

        //public static class Bots
        //{
        //    public const string AngryBot = "AngryBot";
        //}
    }
}
