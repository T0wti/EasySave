namespace EasySave.Domain.Interfaces
{
    public interface ILargeSizeGate
    {
        /// <summary>
        /// Determines whether a file qualifies as "large" based on the configured
        /// size threshold, and therefore requires gate acquisition before copying.
        /// </summary>
        /// <param name="fileSizeBytes">Size of the file in bytes.</param>
        /// <returns>
        /// <c>true</c> if the file meets or exceeds the configured threshold;
        /// <c>false</c> otherwise.
        /// </returns>
        bool IsLargeFile(long fileSizeBytes);

        /// <summary>
        /// Asynchronously acquires the gate if the file qualifies as large,
        /// blocking until a slot is available or the operation is cancelled.
        /// Has no effect if the file does not meet the size threshold.
        /// Must be paired with a corresponding <see cref="ReleaseIfLarge"/> call,
        /// ideally inside a try/finally block.
        /// </summary>
        /// <param name="fileSizeBytes">Size of the file in bytes.</param>
        /// <param name="ct">Token to cancel the wait if the job is stopped.</param>
        Task AcquireIfLargeAsync(long fileSizeBytes, CancellationToken ct);

        /// <summary>
        /// Releases the gate after a large file transfer completes,
        /// allowing another waiting job to proceed.
        /// Has no effect if the file does not meet the size threshold.
        /// </summary>
        /// <param name="fileSizeBytes">Size of the file in bytes.</param>
        void ReleaseIfLarge(long fileSizeBytes);
    }
}
