using Microsoft.Extensions.Logging;

namespace EasyLog.Buisness.Logging;

public class DailyFileLoggerProvider : ILoggerProvider
{
    private readonly DailyFileLoggerOptions _options;

    public readonly DailyFileLoggerProvider(DailyFileLoggerOptions _options)
    {
        _options = options;
    }

    // Initialise un ILogger
    public ILogger CreateLogger(string categoryName)
    {
        new DailyFileLogger(categoryName, _options);
    }
}
