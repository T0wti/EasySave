namespace EasySave.Domain.Interfaces
{
    public interface ICryptoSoftService
    {
        // Encrypts the file at the given path by launching CryptoSoft.exe.
        // Returns: 0 = not encrypted, >0 = duration ms, <0 = error code
        long Encrypt(string filePath);

        // Returns true if the given file extension should be encrypted
        bool ShouldEncrypt(string filePath);
    }
}