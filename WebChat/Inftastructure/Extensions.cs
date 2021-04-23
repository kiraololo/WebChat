namespace WebChat.Inftastructure
{
    public static class Extensions
    {
        public static bool IsEmpty(this string str)
         => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}
