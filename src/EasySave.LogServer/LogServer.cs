using EasySave.EasyLog.Writers;
using EasySave.EasyLog.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

await LogServer.RunAsync();

// TCP log server that receives log entries from EasySave client and writes them to daily log files using EasyLog writers

internal static class LogServer
{
    // Port on which the server listens for incoming TCP connections
    private const int Port = 11000;

    // Directory inside the container where log files are stored (mapped via Docker volume)
    private const string LogDir = "/app/logs";

    // Starts the TCP listener and accepts incoming client connections indefinitely
  
    public static async Task RunAsync()
    {
        // Ensure the log directory exists before any write attempt
        Directory.CreateDirectory(LogDir);

        Console.WriteLine($"[EasySave LogServer] Listening on TCP port {Port}...");

        var listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();

        // Main accept loop runs indefinitely until the process is stopped
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();

            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    /// <summary>
    /// Handles a single connected client:
    /// reads the full payload, detects the format (JSON or XML),
    /// then delegates writing to the appropriate EasyLog writer.
    /// </summary>
    private static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            // Timeout to avoid blocking indefinitely on slow or malformed clients
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var raw = await reader.ReadToEndAsync(cts.Token);

            // Ignore empty payloads
            if (string.IsNullOrWhiteSpace(raw)) return;

            bool isXml;
            string payload;

            // Detect format based on the prefix sent by TcpLogClient
            if (raw.StartsWith("XML|"))
            {
                isXml = true;
                payload = raw["XML|".Length..].TrimEnd('\n');
            }
            else if (raw.StartsWith("JSON|"))
            {
                isXml = false;
                payload = raw["JSON|".Length..].TrimEnd('\n');
            }
            else
            {
                // No recognized prefix : default to JSON
                isXml = false;
                payload = raw.TrimEnd('\n');
            }

            // Instantiate the correct EasyLog writer directly (thread-safe, no singleton issues)
            // JsonLogWriter and XmlLogWriter both have internal locking
            ILogWriter writer = isXml
                ? new XmlLogWriter(LogDir)
                : new JsonLogWriter(LogDir);

            // Write the raw payload string into the daily log file
            writer.Write(payload);

            Console.WriteLine($"[OK] Entry written ({(isXml ? "XML" : "JSON")})");
        }
        catch (OperationCanceledException)
        {
            // Client took too long to send data
            Console.WriteLine("[WARN] Client connection timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
        finally
        {
            // Always close the connection, even on error
            client.Close();
        }
    }
}