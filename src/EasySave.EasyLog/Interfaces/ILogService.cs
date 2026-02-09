namespace EasySave.EasyLog.Interfaces
{
    /// <summary>
    /// Defines a logging service interface.
    /// Acts as the main entry point for the logging system.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Writes a log entry using the configured writer.
        /// </summary>
        /// <param name="entry">Log object to persist.</param>
        void Write(object entry);
    }

}
    