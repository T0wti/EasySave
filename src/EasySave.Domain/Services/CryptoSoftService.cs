using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Diagnostics;

namespace EasySave.Domain.Services
{
    // Launches CryptoSoft.exe as an external process to encrypt a file
    public class CryptoSoftService : ICryptoSoftService
    {
        private readonly ApplicationSettings _settings;

        public CryptoSoftService(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool ShouldEncrypt(string filePath)
        {

            var ext = Path.GetExtension(filePath);
            
            if (_settings.EncryptedFileExtensions == null
                || _settings.EncryptedFileExtensions.Count == 0)
                return false;

            return _settings.EncryptedFileExtensions
                .Any(e => string.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
        }

        public long Encrypt(string filePath)
        {

            if (string.IsNullOrWhiteSpace(_settings.CryptoSoftPath)
                || !File.Exists(_settings.CryptoSoftPath))
                return -1;

            if (string.IsNullOrWhiteSpace(_settings.CryptoSoftKeyPath)
                || !File.Exists(_settings.CryptoSoftKeyPath))
                return -1;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _settings.CryptoSoftPath,
                        ArgumentList = { filePath, _settings.CryptoSoftKeyPath },
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == -10)
                {
                    return -10;
                }

                // CryptoSoft returns duration in ms as exit code, or negative on error
                return process.ExitCode;
            }
            catch
            {
                return -1;
            }
        }
    }
}