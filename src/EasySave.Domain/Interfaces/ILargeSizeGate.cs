namespace EasySave.Domain.Interfaces
{
    public interface ILargeSizeGate
    {
        bool IsLargeFile(long fileSizeBytes);
        Task AcquireIfLargeAsync(long fileSizeBytes, CancellationToken ct);
        void ReleaseIfLarge(long fileSizeBytes);
    }
}
