using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;
using EasySave.EasyLog.Interfaces;

namespace EasySave.EasyLog.Writers
{
    public class XmlLogWriter : ILogWriter
    {
        private readonly string _logDirectory;
        private readonly object _lock = new();

        private static readonly ConcurrentDictionary<Type, XmlSerializer> _serializers = new();

        public XmlLogWriter(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentException("Log directory path cannot be null or empty.", nameof(logDirectory));

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            _logDirectory = logDirectory;
        }

        // Writes a log entry into the daily XML log file
        public void Write<T>(T entry)
        {
            string logFile = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.xml");

            lock (_lock)
            {
                try
                {
                    XmlDocument doc = new();

                    if (File.Exists(logFile))
                    {
                        doc.Load(logFile);
                    }
                    else
                    {
                        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                        doc.AppendChild(doc.CreateElement("LogEntries"));
                    }

                    doc.DocumentElement?.AppendChild(SerializeToXmlElement(entry, doc));

                    // Write to a temp file first to avoid corruption on crash
                    string tempFile = logFile + ".tmp";
                    var settings = new XmlWriterSettings { Indent = true, IndentChars = "  " };

                    using (var writer = XmlWriter.Create(tempFile, settings))
                        doc.Save(writer);

                    File.Move(tempFile, logFile, overwrite: true);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[XmlLogWriter ERROR] {ex.Message}");
                    throw;
                }
            }
        }

        private static XmlElement SerializeToXmlElement<T>(T obj, XmlDocument doc)
        {
            var serializer = _serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t));

            using var memoryStream = new MemoryStream();

            // XMLWriter must be disposed (flushed) before reading the stream
            using (var xmlWriter = XmlWriter.Create(memoryStream))
                serializer.Serialize(xmlWriter, obj);

            memoryStream.Position = 0;

            var tempDoc = new XmlDocument();
            tempDoc.Load(memoryStream);

            return (XmlElement)doc.ImportNode(tempDoc.DocumentElement!, true);
        }
    }
}