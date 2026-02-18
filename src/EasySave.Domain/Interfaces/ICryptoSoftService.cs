namespace EasySave.Domain.Interfaces
{
    public interface ICryptoSoftService
    {
        // Encrypts the file at the given path by launching CryptoSoft.exe.
        long Encrypt(string filePath);

        // Returns true if the given file extension should be encrypted
        bool ShouldEncrypt(string filePath);
    }
}