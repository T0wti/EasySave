namespace EasySave.Domain.Interfaces
{
    //Monitors the business software in the background during backup execution
    //Automatically pauses all jobs when the software is detected, resumes when it stops
    public interface IBusinessSoftwareWatcher
    {
        Task WatchAsync(CancellationToken stopWhen);
    }
}