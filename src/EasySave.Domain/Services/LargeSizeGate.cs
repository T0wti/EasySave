using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{

    // Only one large file can be transferred at a time across all parallel jobs
    // Small files are never blocked
    public class LargeSizeGate : ILargeSizeGate
    {
        private readonly IConfigurationService _configService;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public LargeSizeGate(IConfigurationService configService)
        {
            _configService = configService;
        }

        public bool IsLargeFile(long fileSizeBytes)
        {
        // Reload settings on every call so threshold changes apply immediately.
        var threshold = _configService.LoadSettings().MaxLargeFileSizeKb * 1024;
        return threshold > 0 && fileSizeBytes > threshold; // threshold = 0 means the feature is disabled 
        }

        public async Task AcquireIfLargeAsync(long fileSizeBytes, CancellationToken ct)
        {
            // Small files skip the semaphore and proceed immediately
            if (!IsLargeFile(fileSizeBytes)) return;

            // Block until the previous large file transfer releases the semaphore
            await _semaphore.WaitAsync(ct).ConfigureAwait(false);
        }

        public void ReleaseIfLarge(long fileSizeBytes)
        {
            // Only release if this file actually acquired the semaphore
            if (!IsLargeFile(fileSizeBytes)) return;
            _semaphore.Release();
        }

    }
}
