using System;
using System.Collections.Generic;
using static System.ConsoleColor;

namespace _9xCode
{
    internal static partial class Program
    {
        public static Dictionary<string, ConsoleColor> StringToConsoleColor = new Dictionary<string, ConsoleColor>()
        {
            { "Black", Black }, { "DarkBlue", DarkBlue },
            { "DarkGreen", DarkGreen }, { "DarkCyan", DarkCyan },
            { "DarkRed", DarkRed },{ "DarkMagenta", DarkMagenta },
            { "DarkYellow", DarkYellow }, { "Gray", Gray },
            { "DarkGray", DarkGray },{ "Blue", Blue },
            { "Green", Green },{ "Cyan", Cyan },
            { "Red", Red }, { "Magenta", Magenta },
            { "Yellow", Yellow }, { "White", White },
        };
    }
}
