namespace EasySave.Domain.Interfaces
{
    public interface IPriorityGate
    {
        bool IsPriority(string filePath);
        void RegisterPriorityFiles(int count);
        Task WaitIfNeededAsync(bool isPriority, CancellationToken ct);
        void NotifyPriorityFileCopied();
    }
}