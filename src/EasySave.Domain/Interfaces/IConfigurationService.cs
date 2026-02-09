using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing application configuration settings
    public interface IConfigurationService
    {
        ApplicationSettings LoadSettings();

        void SaveSettings(ApplicationSettings settings);

        bool FileExists();
        void EnsureConfigExists();
    }
}
