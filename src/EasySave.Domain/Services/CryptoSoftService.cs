using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Diagnostics;

namespace EasySave.Domain.Services
{
    // Launches CryptoSoft.exe as an external process to encrypt a file
    public class CryptoSoftService : ICryptoSoftService
    {
        private readonly IConfigurationService _configService;

        // How many times to retry when CryptoSoft reports its mutex is occupied (-10)
        private const int MaxRetries = 10;
        private const int RetryDelayMs = 500;

        public CryptoSoftService(IConfigurationService configService)
        {
            _configService = configService;
        }


        // Reloads settings on every call so extension list changes take effect immediately
        public bool ShouldEncrypt(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;
            var extensions = _configService.LoadSettings().EncryptedFileExtensions ?? new List<string>();
            var ext = Path.GetExtension(filePath);
            return extensions.Any(e => string.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
        }

        public long Encrypt(string sourceFilePath, string targetFilePath)
        {
            var settings = _configService.LoadSettings();

            if (string.IsNullOrWhiteSpace(settings.CryptoSoftPath) || !File.Exists(settings.CryptoSoftPath))
                return -6;

            if (string.IsNullOrWhiteSpace(settings.CryptoSoftKeyPath) || !File.Exists(settings.CryptoSoftKeyPath))
                return -7;

            // Retry loop: CryptoSoft uses a global mutex to prevent concurrent instances
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                long result = RunCryptoSoft(sourceFilePath, targetFilePath, settings.CryptoSoftPath, settings.CryptoSoftKeyPath);

                if (result == -10)
                {
                    Thread.Sleep(RetryDelayMs);
                    continue;
                }

                return result;
            }

            return -10;
        }

        // Launches CryptoSoft.exe, pipes the source file bytes via stdin, reads the encrypted output from stdout, and writes it to the target path
        private static long RunCryptoSoft(string sourceFilePath, string targetFilePath, string cryptoSoftPath, string keyPath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = cryptoSoftPath,
                        ArgumentList = { keyPath },
                        UseShellExecute = false,
                        RedirectStandardInput = true, // We write plain bytes to CryptoSoft
                        RedirectStandardOutput = true, // We read encrypted bytes from CryptoSoft
                        CreateNoWindow = true
                    }
                };
                process.Start();

                var fileBytes = File.ReadAllBytes(sourceFilePath);

                // Write to stdin on a separate task to avoid deadlocking:
                // if the stdout buffer fills up while we block on stdin, both sides stall
                var stdinTask = Task.Run(() =>
                {
                    try
                    {
                        process.StandardInput.BaseStream.Write(fileBytes, 0, fileBytes.Length);
                        process.StandardInput.BaseStream.Close();
                    }
                    catch
                    {
                        // CryptoSoft leave before reading stdin (ex: mutex -10) -> ignore
                    }
                });

                // Drain stdout before waiting for exit to prevent the process from blocking
                byte[] encryptedBytes;
                using (var ms = new MemoryStream())
                {
                    process.StandardOutput.BaseStream.CopyTo(ms);
                    encryptedBytes = ms.ToArray();
                }

                stdinTask.Wait();
                process.WaitForExit();

                int exitCode = process.ExitCode;

                if (exitCode == -10) return -10; // Mutex ococcupied -> retry
                if (exitCode < 0) return exitCode; // Any other negative code = CryptoSoft error

                // Only write the output file after confirming a successful exit code
                var targetDir = Path.GetDirectoryName(targetFilePath);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir!);

                File.WriteAllBytes(targetFilePath, encryptedBytes);
                return exitCode;
            }
            catch (Exception ex)
            {
                return -5;
            }
        }
    }
}