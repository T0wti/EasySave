using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing application configuration settings
    public interface IConfigurationService
    {

        /// <summary>
        /// Reads and deserialises the application settings from the configuration file.
        /// </summary>
        /// <returns>The current <see cref="ApplicationSettings"/> instance.</returns>
        ApplicationSettings LoadSettings();

        /// <summary>
        /// Serialises and persists the provided settings to the configuration file,
        /// overwriting any previously saved values.
        /// </summary>
        /// <param name="settings">The settings object to save.</param>
        void SaveSettings(ApplicationSettings settings);

        /// <summary>
        /// Checks whether the configuration file exists on disk.
        /// Can be used to determine whether first-time initialisation is needed.
        /// </summary>
        /// <returns><c>true</c> if the file exists; <c>false</c> otherwise.</returns>
        bool FileExists();

        /// <summary>
        /// Ensures the configuration file exists, creating it with default values
        /// if it is absent. Should be called at application startup before any
        /// call to <see cref="LoadSettings"/>.
        /// </summary>
        void EnsureConfigExists();

        /// <summary>
        /// Ensures the encryption key file exists, generating and persisting a new key
        /// if none is found. Should be called at startup to guarantee that
        /// <see cref="IFileManager"/> always has a key available.
        /// </summary>
        void EnsureKeyExists();

    }
}
