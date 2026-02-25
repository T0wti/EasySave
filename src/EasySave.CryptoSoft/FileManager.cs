using EasySave.CryptoSoft.Interfaces;
using System.Security.Cryptography;

public class FileManager : IFileManager
{
    private readonly byte[] _keyBytes;

    public FileManager(byte[] keyBytes)
    {
        _keyBytes = keyBytes;
    }

    public byte[] AesEncrypt(byte[] fileBytes, byte[] keyBytes)
    {
        using var aes = Aes.Create();
        using var sha256 = SHA256.Create();

        aes.Key = sha256.ComputeHash(keyBytes);
        aes.GenerateIV();

        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(fileBytes, 0, fileBytes.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }
}