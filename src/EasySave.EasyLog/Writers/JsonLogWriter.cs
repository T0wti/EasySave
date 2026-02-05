using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.EasyLog.Writers
{
    public class JsonLogWriter
    {
        private readonly string _logDirectory;

        public JsonLogWriter(string logDirectory)
        {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            _logDirectory = logDirectory;
        }

        public void WriteJson<T>(T entry)
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

            lock (this)
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
