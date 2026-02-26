namespace EasySave.Domain.Interfaces
{
    // Responsible for detecting whether a configured business software is currently running
    public interface IBusinessSoftwareService
    {
        /// <summary>
        /// Checks whether the configured business software process is currently running.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the process is active; <c>false</c> otherwise.
        /// </returns>
        bool IsBusinessSoftwareRunning();

        /// <summary>
        /// Returns the process name configured by the user for the business software.
        /// Used to build meaningful log entries and exception messages rather than
        /// referring to an anonymous process.
        /// </summary>
        /// <returns>The configured process name (e.g. "sap", "sage").</returns>
        string GetConfiguredName();
    }
}