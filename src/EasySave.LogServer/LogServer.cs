using System.Net;
using System.Net.Sockets;
using System.Text;

await LogServer.RunAsync();

// TCP log server — receives already-serialized log entries from EasySave clients
// and appends them directly to daily log files (JSON or XML).
// Runs inside a Docker container, log files persisted via mounted volume.
internal static class LogServer
{
    private const int Port = 11000;
    private const string LogDir = "/app/logs";

    // One semaphore per file path allows JSON and XML files to be written concurrently
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim>
        _fileLocks = new();

    public static async Task RunAsync()
    {
        Directory.CreateDirectory(LogDir);
        Console.WriteLine($"[EasySave LogServer] Listening on TCP port {Port}...");

        var listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            // Disconnect clients that are too slow or malformed
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var raw = await reader.ReadToEndAsync(cts.Token);

            if (string.IsNullOrWhiteSpace(raw)) return;

            bool isXml;
            string payload;

            // Detect format from prefix set by TcpLogClient
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
                isXml = false;
                payload = raw.TrimEnd('\n');
            }

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var filePath = Path.Combine(LogDir, $"{today}.{(isXml ? "xml" : "json")}");

            var fileLock = _fileLocks.GetOrAdd(filePath, _ => new SemaphoreSlim(1, 1));

            if (isXml)
                await AppendXmlAsync(filePath, payload, fileLock);
            else
                await AppendJsonAsync(filePath, payload, fileLock);

            Console.WriteLine($"[OK] Entry written ({(isXml ? "XML" : "JSON")}) → {Path.GetFileName(filePath)}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[WARN] Client connection timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    // Inserts a JSON object into the daily JSON array file
    private static async Task AppendJsonAsync(string filePath, string entry, SemaphoreSlim fileLock)
    {
        await fileLock.WaitAsync();
        try
        {
            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(filePath, "[\n" + entry + "\n]");
                return;
            }

            var content = await File.ReadAllTextAsync(filePath);
            content = content.TrimEnd();
            if (content.EndsWith("]"))
                content = content[..^1].TrimEnd();

            await File.WriteAllTextAsync(filePath, content + ",\n" + entry + "\n]");
        }
        finally
        {
            fileLock.Release();
        }
    }

    // Inserts an XML element into the daily XML file inside <LogEntries>
    private static async Task AppendXmlAsync(string filePath, string entry, SemaphoreSlim fileLock)
    {
        await fileLock.WaitAsync();
        try
        {
            // Strip XML declaration if present (<?xml version="1.0"?>)
            var clean = entry.Trim();
            if (clean.StartsWith("<?xml"))
                clean = clean[(clean.IndexOf("?>") + 2)..].TrimStart();

            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(filePath, "<LogEntries>\n" + clean + "\n</LogEntries>");
                return;
            }

            var content = await File.ReadAllTextAsync(filePath);
            var insertPos = content.LastIndexOf("</LogEntries>");
            if (insertPos < 0)
            {
                await File.AppendAllTextAsync(filePath, clean + "\n");
                return;
            }

            await File.WriteAllTextAsync(filePath,
                content[..insertPos] + clean + "\n</LogEntries>");
        }
        finally
        {
            fileLock.Release();
        }
    }
}