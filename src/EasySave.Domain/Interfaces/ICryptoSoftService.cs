namespace EasySave.Domain.Interfaces
{
    public interface ICryptoSoftService
    {
        /// <summary>
        /// Encrypts a file by launching the CryptoSoft.exe process with the given paths.
        /// Blocks until the process exits and returns its exit code.
        /// </summary>
        /// <param name="sourceFilePath">Full path of the plain-text file to encrypt.</param>
        /// <param name="targetFilePath">Full path where the encrypted output should be written.</param>
        /// <returns>
        /// The exit code returned by CryptoSoft.exe.
        /// A value of <c>0</c> indicates success; any other value indicates failure
        /// and should be wrapped in a <see cref="CryptoSoftException"/>.
        /// </returns>
        long Encrypt(string sourceFilePath, string targetFilePath);

        /// <summary>
        /// Determines whether the file at the given path should be encrypted,
        /// based on the list of extensions configured by the user.
        /// </summary>
        /// <param name="filePath">Full path or filename of the file to check.</param>
        /// <returns>
        /// <c>true</c> if the file's extension matches a configured encryption target;
        /// <c>false</c> otherwise.
        /// </returns>
        bool ShouldEncrypt(string filePath);
    }
}