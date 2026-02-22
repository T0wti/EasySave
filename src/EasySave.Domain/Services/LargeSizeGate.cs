using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{

    // Only one large file can be transferred at a time across all parallel jobs
    // Small files are never blocked
    public class LargeSizeGate : ILargeSizeGate
    {
        private readonly long _thresholdBytes;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public LargeSizeGate(long maxLargeFileSizeKb)
        {
            _thresholdBytes = maxLargeFileSizeKb * 1024;
        }

        public bool IsLargeFile(long fileSizeBytes)
        => _thresholdBytes > 0 && fileSizeBytes > _thresholdBytes;

        public async Task AcquireIfLargeAsync(long fileSizeBytes, CancellationToken ct)
        {
            if (!IsLargeFile(fileSizeBytes)) return;
            await _semaphore.WaitAsync(ct).ConfigureAwait(false);
        }

        public void ReleaseIfLarge(long fileSizeBytes)
        {
            if (!IsLargeFile(fileSizeBytes)) return;
            _semaphore.Release();
        }

    }
}
