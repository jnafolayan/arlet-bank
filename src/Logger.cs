using System;
using Sharprompt;

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

        /// <param name="level">The logger level</param>
        public Logger(Logger.Levels level)
        {
            Level = level;
        }

        /// <summary>
        /// Debug log
        /// </summary>
        /// <param name="msg">The output string</param>
        public void Debug(string msg)
        {
            if (Level >= Logger.Levels.DEBUG)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Info log
        /// </summary>
        /// <param name="msg">The output string</param>
        public void Info(string msg)
        {
            if (Level >= Logger.Levels.INFO)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Warn log
        /// </summary>
        /// <param name="msg">The output string</param>
        public void Warn(string msg)
        {
            if (Level >= Logger.Levels.WARN)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="msg">The output string</param>
        public void Error(string msg)
        {
            if (Level >= Logger.Levels.ERROR)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{msg}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Success log
        /// </summary>
        /// <param name="msg">The output string</param>
        public void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Asks a question on the terminal and waits for input
        /// </summary>
        /// <param name="question">The question string</param>
        /// <param name="defaultValue">The default answer</param>
        public T Question<T>(string question, T defaultValue) 
        {
            T answer = Prompt.Input<T>(question, defaultValue);
            return answer;
        }

        /// <summary>
        /// Asks a question on the terminal and waits for input
        /// </summary>
        /// <param name="question">The question string</param>
        public T Question<T>(string question) 
        {
            T answer = Prompt.Input<T>(question);
            return answer;
        }

        /// <summary>
        /// Asks a yes/no question on the terminal and waits for input
        /// </summary>
        /// <param name="question">The question string</param>
        public bool Confirm(string question)
        {
            return Prompt.Confirm(question);
        }

        public string Password(string question)
        {
            return Prompt.Password(question);
        }

        /// <summary>
        /// Asks for a password and waits for input
        /// </summary>
        /// <param name="question">The question string</param>
        /// <param name="options">A list of options for the user to pick from</param>
        public string Select(string question, string[] options)
        {
            return Prompt.Select(question, options);
        }

        public Logger.Levels Level { get; private set; }

        public enum Levels { ERROR, WARN, INFO, DEBUG }
    }
}