namespace EasySave.Domain.Enums
{
    // Stored as int in ApplicationSettings, same pattern as LogFormat
    public enum LogMode
    {
        Local = 0,
        Centralized = 1,
        Both = 2
    }
}