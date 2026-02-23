using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Diagnostics;

namespace EasySave.Domain.Services
{
    // Launches CryptoSoft.exe as an external process to encrypt a file
    public class CryptoSoftService : ICryptoSoftService
    {
        private readonly IConfigurationService _configService;

        private const int MaxRetries = 10;
        private const int RetryDelayMs = 500;

        public CryptoSoftService(IConfigurationService configService)
        {
            _configService = configService;
        }


        // Reads extensions fresh from config on every call
        public bool ShouldEncrypt(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;
            var extensions = _configService.LoadSettings().EncryptedFileExtensions ?? new List<string>();
            var ext = Path.GetExtension(filePath);
            return extensions.Any(e => string.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
        }

        public long Encrypt(string filePath)
        {
            var settings = _configService.LoadSettings();

            if (string.IsNullOrWhiteSpace(settings.CryptoSoftPath) || !File.Exists(settings.CryptoSoftPath))
                return -1;

            if (string.IsNullOrWhiteSpace(settings.CryptoSoftKeyPath) || !File.Exists(settings.CryptoSoftKeyPath))
                return -1;

            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                long result = RunCryptoSoft(filePath, settings.CryptoSoftPath, settings.CryptoSoftKeyPath);

                if (result == -10)
                {
                    Thread.Sleep(RetryDelayMs);
                    continue;
                }

                return result;
            }

            return -10;
        }

        private static long RunCryptoSoft(string filePath, string cryptoSoftPath, string keyPath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = cryptoSoftPath,
                        ArgumentList = { filePath, keyPath! },
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