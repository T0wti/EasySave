namespace EasySave.Domain.Interfaces
{
    // Controls the execution flow of a single running backup job

    public interface IBackupJobHandle : IDisposable
    {
        bool IsPaused { get; }
        void WaitIfPaused();
        void Pause();
        void Resume();
        void Stop();

        CancellationToken CancellationToken { get; }
    }
}