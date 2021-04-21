using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChat.Inftastructure
{
    public static class Extensions
    {
        public static bool IsEmpty(this string str)
         => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}
