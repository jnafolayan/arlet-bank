using System;

namespace ArletBank
{
    /// <summary>
    /// Logger wraps functionality to write to stdout / stderr
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Parameterless constructor instantiates a new logger with the debug 
        /// level
        /// </summary>
        public Logger() : this(Logger.Levels.DEBUG)
        {}

        public Logger(Logger.Levels level)
        {
            Level = level;
        }

        public void Debug(string msg)
        {
            if (Level >= Logger.Levels.DEBUG)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        public void Info(string msg)
        {
            if (Level >= Logger.Levels.INFO)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        public void Warn(string msg)
        {
            if (Level >= Logger.Levels.WARN)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        public void Error(string msg)
        {
            if (Level >= Logger.Levels.ERROR)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        public Logger.Levels Level { get; private set; }

        public enum Levels { ERROR, WARN, INFO, DEBUG }
    }
}