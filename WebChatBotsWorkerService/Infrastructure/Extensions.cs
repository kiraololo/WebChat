namespace WebChatBotsWorkerService.Infrastructure
{
    public static class Extensions
    {
        public static int ToInt(this string str, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }
            var res = defaultValue;
            int.TryParse(str, out res);
            return res;
        }
    }
}
