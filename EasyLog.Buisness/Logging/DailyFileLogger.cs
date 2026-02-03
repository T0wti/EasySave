using System.Text.Json;
using Microsoft.Extensions.Logging;
using EasyLog.Buisness.Models;

namespace EasyLog.Buisness.Logging;

public class DailyFileLogger : ILogger
{
    private JsonSerializerOptions _jsonSerializerOptions;
    private readonly DailyFileLoggerOptions _options;

    public DailyFileLogger()
    {

    }
    
    public bool isEnable(LogLevel logLevel)
    {

    }  
    
    public void Log<TState>
        (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState,Exception, string> formatter
        )
    {
        if (!isEnable(logLevel))
        {
            return;
        }
        string fileName = $"{DateTime.Now:yyyy-MM-dd}.json";
        // string filePath
        string json = _jsonSerializerOptions();
    }
