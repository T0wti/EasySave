using EasySave.CryptoSoft.Interfaces;
using System.Security.Cryptography;

// Handles low-level cryptographic file operations
public class FileManager : IFileManager
{
    private readonly byte[] _keyBytes;

    public FileManager(byte[] keyBytes)
    {
        _keyBytes = keyBytes;
    }

    // Encrypts a byte array using AES-256 in CBC mode. 
    public byte[] AesEncrypt(byte[] fileBytes, byte[] keyBytes)
    {
        using var aes = Aes.Create();
        using var sha256 = SHA256.Create();

        // Derive a 256-bit AES key from the raw key material via SHA-256
        aes.Key = sha256.ComputeHash(keyBytes);

        // Generate a fresh random IV for every encryption call (CBC best practice)
        aes.GenerateIV();

        using var ms = new MemoryStream();

        // Prepend the IV in plain text so the decryptor can read it back
        ms.Write(aes.IV, 0, aes.IV.Length);

        // Encrypt and stream the cipher text directly into the same MemoryStream
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(fileBytes, 0, fileBytes.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }
}