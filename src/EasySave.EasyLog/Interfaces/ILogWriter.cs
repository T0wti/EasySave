namespace EasySave.EasyLog.Interfaces
{
    /// <summary>
    /// Defines a generic log writer capable of persisting log entries.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="T">Type of the log entry.</typeparam>
        /// <param name="entry">The log object to write.</param>
        void Write<T>(T entry);
    }
}