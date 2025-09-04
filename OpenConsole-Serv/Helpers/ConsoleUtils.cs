using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenConsole.Helpers
{
    internal class ConsoleUtils
    {
        private static readonly object ConsoleLock = new object();
        public static void PrintLogColored(string input)
        {
            lock (ConsoleLock)
            {
                string pattern = "<color=(#[0-9a-fA-F]{6}|\\w+)>";
                int startIndex = 0;
                ConsoleColor consoleColor = ConsoleColor.Gray;
                MatchCollection matches = Regex.Matches(input, pattern);
                if (matches.Count == 0)
                {
                    Console.ForegroundColor = consoleColor;
                    Console.WriteLine(input);
                    Console.ResetColor();
                    return;
                }
                foreach (Match match in matches)
                {
                    if (startIndex < match.Index)
                    {
                        Console.ForegroundColor = consoleColor;
                        Console.Write(input.Substring(startIndex, match.Index - startIndex));
                    }
                    string colorCode = match.Groups[1].Value;
                    if (Enum.TryParse(colorCode, true, out ConsoleColor namedColor))
                    {
                        consoleColor = namedColor;
                    }
                    else if (Regex.IsMatch(colorCode, "^#[0-9a-fA-F]{6}$"))
                    {
                        consoleColor = ClosestConsoleColor(colorCode);
                    }
                    startIndex = match.Index + match.Length;
                }
                if (startIndex < input.Length)
                {
                    Console.ForegroundColor = consoleColor;
                    Console.Write(input.Substring(startIndex));
                }
                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private static ConsoleColor ClosestConsoleColor(string hex)
        {
            int r = Convert.ToInt32(hex.Substring(1, 2), 16);
            int g = Convert.ToInt32(hex.Substring(3, 2), 16);
            int b = Convert.ToInt32(hex.Substring(5, 2), 16);
            ConsoleColor bestColor = ConsoleColor.Gray;
            double bestDistance = double.MaxValue;
            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var (cr, cg, cb) = ColorFromConsoleColor(cc);
                double distance = Math.Pow(cr - r, 2) + Math.Pow(cg - g, 2) + Math.Pow(cb - b, 2);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestColor = cc;
                }
            }
            return bestColor;
        }

        private static readonly Dictionary<ConsoleColor, (int R, int G, int B)> ConsoleColorMap =
            new Dictionary<ConsoleColor, (int, int, int)>
            {
                { ConsoleColor.Black,      (0, 0, 0) },
                { ConsoleColor.DarkBlue,   (0, 0, 128) },
                { ConsoleColor.DarkGreen,  (0, 128, 0) },
                { ConsoleColor.DarkCyan,   (0, 128, 128) },
                { ConsoleColor.DarkRed,    (128, 0, 0) },
                { ConsoleColor.DarkMagenta,(128, 0, 128) },
                { ConsoleColor.DarkYellow, (128, 128, 0) },
                { ConsoleColor.Gray,       (192, 192, 192) },
                { ConsoleColor.DarkGray,   (128, 128, 128) },
                { ConsoleColor.Blue,       (0, 0, 255) },
                { ConsoleColor.Green,      (0, 255, 0) },
                { ConsoleColor.Cyan,       (0, 255, 255) },
                { ConsoleColor.Red,        (255, 0, 0) },
                { ConsoleColor.Magenta,    (255, 0, 255) },
                { ConsoleColor.Yellow,     (255, 255, 0) },
                { ConsoleColor.White,      (255, 255, 255) }
            };

        private static (int R, int G, int B) ColorFromConsoleColor(ConsoleColor c)
        {
            if (ConsoleColorMap.TryGetValue(c, out var rgb))
                return rgb;
            return (192, 192, 192);
        }
    }
    internal class CLog
    {
        public static void Log(string a, string b)
        {
            ConsoleUtils.PrintLogColored($"<color={vars.TimestartendColor}>[<color={vars.TimeColor}>{DateTime.Now.ToShortTimeString()}<color={vars.TimestartendColor}>]:[<color={vars.ModuleColor}>{a}<color={vars.TimestartendColor}>]:{b}");
        }

        public static void LogG(string text)
        {
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                int gray = 170 - (i * 170 / Math.Max(length - 1, 1));
                int green = gray;
                int blue = gray + 5;
                gray = Math.Max(0, Math.Min(255, gray));
                green = Math.Max(0, Math.Min(255, green));
                blue = Math.Max(0, Math.Min(255, blue));
                Console.Write($"\x1b[38;2;{gray};{green};{blue}m{text[i]}");
            }
            Console.WriteLine("\x1b[0m");
        }
    }
}
