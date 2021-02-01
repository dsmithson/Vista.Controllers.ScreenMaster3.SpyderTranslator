using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Console
{
    public static class StringExtensions
    {
        public static int IndexOf(this string[] values, string value, bool ignoreCase)
        {
            if (values != null)
            {
                int length = values.Length;
                for (int i = 0; i < length; i++)
                {
                    if (string.Compare(values[i], value, ignoreCase) == 0)
                        return i;
                }
            }
            return -1;
        }

        public static string ClipOrPad(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty.PadRight(length);
            else if (text.Length < length)
                return text.PadRight(length);
            else
                return text.Substring(0, length);
        }
    }
}
