using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace DragonEngineLibrary
{
    public class Logger
    {
        /// <summary>
        /// Represents a logged message. This information includes a source, creation timestamp, event type and message.
        /// </summary>
        public struct LogMessage
        {
            /// <summary>
            /// The source of this log.
            /// </summary>
            public string Source;
            
            /// <summary>
            /// The creation time of this log.
            /// </summary>
            public DateTime Time;

            /// <summary>
            /// The event type of this log.
            /// </summary>
            public Event Event;

            /// <summary>
            /// The message of this log.
            /// </summary>
            public string Message;


            /// <summary>
            /// Returns the log's information in the following format: Timestamp (HH:mm:ss.fff) | Event | Source
            /// </summary>
            public string GetLogInfoString()
            {
                return $"{Time.ToString("HH:mm:ss.fff")} | {EventStrings[(int)Event]} | [{Source}]";
            }

            /// <summary>
            /// Returns this log's message preceded by its timestamp, event type and source.
            /// </summary>
            public override string ToString()
            {
                return $"{GetLogInfoString()} {Message}";
            }
        }


        /// <summary>
        /// Contains all logs in order of submission.
        /// </summary>
        public static volatile ConcurrentQueue<LogMessage> Logs = new ConcurrentQueue<LogMessage>();


        /// <summary>
        /// Event type.
        /// </summary>
        public enum Event
        {
            DEBUG,
            INFORMATION,
            WARNING,
            ERROR,
            FATAL,
        }


        /// <summary>
        /// Shortened name for each <see cref="Event"/>.
        /// </summary>
        private static List<string> EventStrings = new List<string>()
        {
            "DBG",
            "INF",
            "WRN",
            "ERR",
            "FTL",
        };


        /// <summary>
        /// Event colors in ABGR format for ImGui.
        /// </summary>
        private static List<uint> EventColorsABGR = new List<uint>()
        {
            0x80333333, // DEBUG
            0x00000000, // INFORMATION
            0x800070EE, // WARNING
            0x800000EE, // ERROR
            0x80BF0898, // FATAL
        };


        /// <summary>
        /// Event colors for consoles/terminals.
        /// </summary>
        private static List<ConsoleColor> EventColorsConsole = new List<ConsoleColor>()
        {
            ConsoleColor.Gray,      // DEBUG
            ConsoleColor.White,     // INFORMATION
            ConsoleColor.Yellow,    // WARNING
            ConsoleColor.Red,       // ERROR
            ConsoleColor.Magenta,   // FATAL
        };


        /// <summary>
        /// Gets the specified <see cref="Event"/>'s color in ABGR format to be used in ImGui.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <returns>An ABGR color as <see cref="uint"/>.</returns>
        public static uint GetABGRColorForLogEventType(Event eventType)
        {
            return EventColorsABGR[(int)eventType];
        }


        /// <summary>
        /// Gets the specified <see cref="Event"/>'s color as <see cref="ConsoleColor"/> to be used in a terminal.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <returns>The color as <see cref="ConsoleColor"/>.</returns>
        public static ConsoleColor GetConsoleColorForLogEventType(Event eventType)
        {
            return EventColorsConsole[(int)eventType];
        }


        /// <summary>
        /// Logs an event with its corresponding message and source.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the log.</param>
        /// <param name="eventType">The event type.</param>
        internal static void LogEvent(string message, string source, Event eventType)
        {
            LogMessage logMessage = new LogMessage()
            {
                Source = source,
                Time = DateTime.Now,
                Event = eventType,
                Message = message
            };
            Logs.Enqueue(logMessage);

            string logMessageString = logMessage.ToString();
            try
            {
                File.AppendAllText("de_log.txt", logMessageString + "\n");
            }
            catch { }
            Console.ForegroundColor = GetConsoleColorForLogEventType(eventType);
            Console.WriteLine(logMessageString);
            Console.ResetColor();
        }


        /// <summary>
        /// Gets a list of logs filtered by source and event type.
        /// </summary>
        /// <param name="source">The source to filter for. A null or empty value will include all sources.</param>
        /// <param name="logEvents">The event types to filter for. A null or empty value will include all event types.</param>
        /// <returns>A <see cref="LogMessage"/> list.</returns>
        public static List<LogMessage> GetFilteredLogs(string source, List<Event> logEvents)
        {
            var filtered = new List<LogMessage>();

            foreach (var logMessage in Logs)
            {
                if (!string.IsNullOrEmpty(source) && logMessage.Source != source)
                    continue;

                if (logEvents == null || logEvents.Count == 0 || logEvents.Contains(logMessage.Event))
                    filtered.Add(logMessage);
            }

            return filtered;
        }
    }
}
