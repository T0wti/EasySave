namespace EasySave.Domain.Interfaces
{
    public interface ILogClient
    {

        /// <summary>
        /// Asynchronously sends a log entry to the remote destination.
        /// Implementations are responsible for serialising the entry
        /// into the appropriate format (ex : JSON, XML) and handling
        /// connection failures, retries, or fallback behaviour.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the log entry. Kept generic to allow different
        /// entry shapes (e.g. backup log, state update) to share the same transport.
        /// </typeparam>
        /// <param name="entry">The log entry object to transmit.</param>
        Task SendAsync<T>(T entry);
    }
}
