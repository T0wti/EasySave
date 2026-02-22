using EasySave.Domain.Interfaces;

namespace EasySave.Domain.Services
{
    // Gate that enforces priority file ordering across all parallel jobs
    public class PriorityGate : IPriorityGate
    {

        private readonly IEnumerable<string> _priorityExtensions;

        // Counts how many priority files are still waiting to be copied across all active jobs combined
        private int _pendingPriorityCount = 0;

        // Released (set) when _pendingPriorityCount reaches 0
        private readonly SemaphoreSlim _gate = new(1, 1);
        private readonly object _lock = new();  

        public PriorityGate(IEnumerable<string> priorityExtensions)
        {
            _priorityExtensions = priorityExtensions;
        }

        public bool IsPriority(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            return _priorityExtensions.Any(p =>
                string.Equals(p, ext, StringComparison.OrdinalIgnoreCase));
        }

        // Count the number of priority file
        public void RegisterPriorityFiles(int count)
        {
            if (count <= 0) return;

            lock (_lock)
            {
                if (_pendingPriorityCount == 0)
                {
                    // First priority files arriving : close the gate = no priority files start waiting
                    _gate.Wait(); // gate closed
                }
                _pendingPriorityCount += count;
            }
        }

        // Block the no priority files
        public async Task WaitIfNeededAsync(bool isPriority, CancellationToken ct)
        {
            // Priority files never wait and dont pass by the semaphore
            if (isPriority) return;

            // Non-priority: wait until gate is open 
            await _gate.WaitAsync(ct).ConfigureAwait(false);
            _gate.Release(); // Release to avoid a lock bc the non priority job pass by the semaphore
        }

        // Decrease the count when a priority file is copied
        public void NotifyPriorityFileCopied()
        {
            lock (_lock)
            {
                if (_pendingPriorityCount <= 0) return;

                _pendingPriorityCount--;

                if (_pendingPriorityCount == 0)
                {
                    // Last priority file done: open the gate
                    _gate.Release();
                }
            }
        }
    }
}