using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Diagnostics;

namespace EasySave.Domain.Services
{
    // Launches CryptoSoft.exe as an external process to encrypt a file
    public class CryptoSoftService : ICryptoSoftService
    {
        private readonly ApplicationSettings _settings;

        private const int MaxRetries = 50;
        private const int RetryDelayMs = 500;

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

            // Retry loop : CryptoSoft is mono-instance and returns -10 when busy
            // We wait and retry instead of failing the whole backup
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                long result = RunCryptoSoft(filePath);

                if (result == -10)
                {
                    // CryptoSoft busy : wait and retry
                    Thread.Sleep(RetryDelayMs);
                    continue;
                }

                // Any other result (success or real error) → return immediately
                return result;
            }

            // All retries exhausted : CryptoSoft was busy too long
            return -10;
        }

        private long RunCryptoSoft(string filePath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _settings.CryptoSoftPath,
                        ArgumentList =
                        {
                            filePath,
                            _settings.CryptoSoftKeyPath
                        },
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode;
            }
            catch
            {
                return -1;
            }
        }
    }
}