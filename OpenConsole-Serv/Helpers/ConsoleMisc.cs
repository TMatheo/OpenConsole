using System;
using System.Runtime.InteropServices;

namespace OpenConsole.Helpers
{
    internal class ConsoleMisc
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        public static void Setup()
        {
            Console.CursorVisible = false;
            IntPtr handle = GetStdHandle(-11);
            if (!GetConsoleMode(handle, out int mode))
            {
                Console.WriteLine("Failed to get console mode.");
                return;
            }
            mode |= 0x0004;
            if (!SetConsoleMode(handle, mode))
            {
                Console.WriteLine("Failed to enable VT processing.");
            }
            Console.Title = vars.Title;
            CLog.LogG(vars.ASCIIArt);
        }
    }
}
