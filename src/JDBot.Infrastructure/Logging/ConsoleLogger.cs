using System;
using JDBot.Infrastructure.Framework;

namespace JDBot.Infrastructure.Logging
{
	public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine($"[JDBot] {message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"[JDBot] {message}");
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[JDBot] {message}");
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"[JDBot] {message}");
            Console.ResetColor();
        }
    }
}
