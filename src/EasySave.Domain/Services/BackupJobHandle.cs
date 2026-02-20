using EasySave.Domain.Interfaces;

namespace EasySave.Domain.Models
{
    // Concrete handle over a single running backup job.
    // Permite pause (ManualResetEventSlim) and stop (CancellationTokenSource).
    public class BackupJobHandle : IBackupJobHandle
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(); // Give the order to stop : the thread does not exist anymore
        private readonly ManualResetEventSlim _mres = new(initialState: true); // Pause (block) a thread until further notice but the thread always exist

        public CancellationToken CancellationToken => _cts.Token; // Listen the order to stop
        public bool IsPaused => !_mres.IsSet;

        public void WaitIfPaused() => _mres.Wait(_cts.Token); // Wait until a set has been execute or a stop is execute

        public void Pause() => _mres.Reset();

        public void Resume() => _mres.Set();

        public void Stop()
        {
            _mres.Set(); // unblock WaitIfPaused before cancelling
            _cts.Cancel();
        }

        public void Dispose() // Free system resources to avoid memory leak
        {
            _cts.Dispose();
            _mres.Dispose();
        }
    }
}