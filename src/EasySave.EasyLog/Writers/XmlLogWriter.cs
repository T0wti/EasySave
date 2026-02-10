using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using EasySave.EasyLog.Interfaces;

namespace EasySave.EasyLog.Writers
{
    public class XmlLogWriter : ILogWriter
    {
        private readonly string _logDirectory;
        private readonly object _lock = new();

        public XmlLogWriter(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentException("Log directory path cannot be null or empty.", nameof(logDirectory));

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            _logDirectory = logDirectory;
        }

        public void Write<T>(T entry)
        {
            string logFile = Path.Combine(
                _logDirectory,
                $"{DateTime.Now:yyyy-MM-dd}.xml"
            );

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
                        XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                        doc.AppendChild(declaration);
                        XmlElement root = doc.CreateElement("LogEntries");
                        doc.AppendChild(root);
                    }

                    XmlElement entryElement = SerializeToXmlElement(entry, doc);
                    doc.DocumentElement?.AppendChild(entryElement);

                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  "
                    };

                    using var writer = XmlWriter.Create(logFile, settings);
                    doc.Save(writer);
                }
                catch (Exception ex)
                {
                    // ✅ AJOUT : Afficher l'erreur pour debug
                    System.Console.WriteLine($"[XmlLogWriter ERROR] {ex.Message}");
                    System.Console.WriteLine($"Stack: {ex.StackTrace}");
                    throw; // Re-throw pour ne pas masquer l'erreur
                }
            }
        }

        private XmlElement SerializeToXmlElement<T>(T obj, XmlDocument doc)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                using var memoryStream = new MemoryStream();
                using var xmlWriter = XmlWriter.Create(memoryStream);

                serializer.Serialize(xmlWriter, obj);
                memoryStream.Position = 0;

                var tempDoc = new XmlDocument();
                tempDoc.Load(memoryStream);

                return (XmlElement)doc.ImportNode(tempDoc.DocumentElement!, true);
            }
            catch (Exception ex)
            {
                // ✅ AJOUT : Afficher l'erreur de sérialisation
                System.Console.WriteLine($"[XML Serialization ERROR for type {typeof(T).Name}] {ex.Message}");
                if (ex.InnerException != null)
                    System.Console.WriteLine($"Inner: {ex.InnerException.Message}");
                throw;
            }
        }
    }
}