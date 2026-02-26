namespace EasySave.Domain.Interfaces
{
    public interface IPriorityGate
    {
        /// <summary>
        /// Determines whether the given file qualifies as a priority file
        /// based on its extension and the configured priority extension list.
        /// </summary>
        /// <param name="filePath">Full path or filename of the file to check.</param>
        /// <returns>
        /// <c>true</c> if the file's extension matches a configured priority extension;
        /// <c>false</c> otherwise.
        /// </returns>
        bool IsPriority(string filePath);

        /// <summary>
        /// Registers the total number of priority files that must be copied
        /// before non priority files are released.
        /// Should be called once per job before execution begins,
        /// after the file list has been determined.
        /// </summary>
        /// <param name="count">Total number of priority files in this backup run.</param>
        void RegisterPriorityFiles(int count);

        /// <summary>
        /// Asynchronously blocks the caller if it is a non priority file
        /// and priority files are still pending.
        /// Priority files pass through immediately without waiting.
        /// </summary>
        /// <param name="isPriority">
        /// Whether the file about to be copied is a priority file.
        /// Typically the result of a prior <see cref="IsPriority"/> call.
        /// </param>
        /// <param name="ct">Token to cancel the wait if the job is stopped.</param>
        Task WaitIfNeededAsync(bool isPriority, CancellationToken ct);

        /// <summary>
        /// Signals that one priority file has been successfully copied.
        /// Decrements the internal counter and, when it reaches zero,
        /// releases all non-priority files that are waiting at <see cref="WaitIfNeededAsync"/>.
        /// </summary>
        void NotifyPriorityFileCopied();
    }
}