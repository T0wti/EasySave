using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using EasySave.EasyLog.Interfaces;

namespace EasySave.EasyLog.Writers
{
    // Writes log entries in JSON format
    // One file is created per day, containing an array of log objects
    public class JsonLogWriter : ILogWriter
    {
        private readonly string _logDirectory;
        private readonly object _lock = new();

        // Initializes a new JSON log writer
        public JsonLogWriter(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentException("Log directory path cannot be null or empty.", nameof(logDirectory));

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            _logDirectory = logDirectory;
        }

        // Writes a log entry into the daily JSON log file
        public void Write<T>(T entry)
        {
            string logFile = Path.Combine(
                _logDirectory,
                $"{DateTime.Now:yyyy-MM-dd}.json"
            );

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(entry, options);

            lock (_lock)
            {
                if (!File.Exists(logFile))
                {
                    File.WriteAllText(logFile, "[\n" + json + "\n]");
                }
                else
                {
                    var content = File.ReadAllText(logFile);
                    content = content.TrimEnd();

                    if (content.EndsWith("]"))
                        content = content.Substring(0, content.Length - 1);

                    content += ",\n" + json + "\n]";
                    File.WriteAllText(logFile, content);
                }
            }
        }
    }
}