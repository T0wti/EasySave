using EasySave.Domain.Enums;

namespace EasySave.Domain.Exceptions
{
    // Thrown when a backup job cannot start or must stop because the configured business software is currently running
    public class BusinessSoftwareRunningException : EasySaveException
    {
        public string SoftwareName { get; }

        public BusinessSoftwareRunningException(string softwareName)
            : base(EasySaveErrorCode.BusinessSoftwareRunning)
        {
            SoftwareName = softwareName;
        }
    }

}