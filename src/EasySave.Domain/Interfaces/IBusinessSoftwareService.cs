namespace EasySave.Domain.Interfaces
{
    // Responsible for detecting whether a configured business software is currently running
    public interface IBusinessSoftwareService
    {
        // Returns true if the configured business software process is currently running
        bool IsBusinessSoftwareRunning();

        // Returns the configured process name, for use in exception messages and logs
        string GetConfiguredName();
    }
}