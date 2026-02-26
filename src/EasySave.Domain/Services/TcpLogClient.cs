using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace EasySave.Domain.Services
{
    // Sends log entries to a remote TCP log server (ex : the Docker LogServer)
    public class TcpLogClient : ILogClient
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _isXml;
        private readonly string _fallbackDirectory;

        private static readonly ConcurrentDictionary<Type, XmlSerializer> _xmlSerializers = new();

        public TcpLogClient(string host, int port, bool isXml, string fallbackDirectory)
        {
            _host = host;
            _port = port;
            _isXml = isXml;
            _fallbackDirectory = fallbackDirectory;
        }

        // Serializes the log entry and sends it to the remote TCP server
        // The payload is prefixed with "XML|" or "JSON|" so the server knows which format to use when writing to the log file
        public async Task SendAsync<T>(T entry)
        {
            try
            {
                // Serialize the entry in the configured format
                var serialized = _isXml
                    ? SerializeXml(entry!, entry!.GetType())
                    : SerializeJson(entry);

                // Prefix tells the server which format this payload is in
                var tag = _isXml ? "XML|" : "JSON|";
                var payload = Encoding.UTF8.GetBytes(tag + serialized + "\n");

                // Open a fresh TCP connection, send the payload, then close
                using var client = new TcpClient();
                await client.ConnectAsync(_host, _port).ConfigureAwait(false);
                await using var stream = client.GetStream();
                await stream.WriteAsync(payload).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new LogServerUnavailableException(_host, _port, ex);
            }
        }

        // Serializes a log entry to an indented JSON string
        // Null properties are omitted
        private static string SerializeJson<T>(T entry)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(entry, options);
        }

        // Serializes a log entry to an indented XML string
        // Uses the runtime type so XmlSerializer can resolve the concrete class and its properties correctly
        private static string SerializeXml(object entry, Type type)
        {
            var serializer = _xmlSerializers.GetOrAdd(type, t => new XmlSerializer(t));

            using var sw = new StringWriter();
            using var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, IndentChars = "  " });
            serializer.Serialize(xw, entry);

            return sw.ToString();
        }
    }
}