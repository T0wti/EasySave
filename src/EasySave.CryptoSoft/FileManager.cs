using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using EasySave.CryptoSoft.Interfaces;

namespace EasySave.CryptoSoft
{
    public class FileManager : IFileManager
    {
        private readonly string _fileName;
        private readonly string _keyPath; 
        public FileManager(string fileName, string keyPath) 
        {
            _fileName = fileName;
            _keyPath = keyPath;
        }

        // CheckFile() returns true if fileName and keyPath exist
        public bool CheckFile()
        {
            return File.Exists(_fileName) && File.Exists(_keyPath);
        }

        /*
         * EncryptFile() reads the file and key
         * Encrypts the file using AesEncrypt()
         * Writes the encrypted file
         * Returns the encryption time in ms
        */
        public int EncryptFile()
        {
            if (!CheckFile()) return -1;

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var fileBytes = File.ReadAllBytes(_fileName);
                byte[] keyBytes = File.ReadAllBytes(_keyPath);
                var encryptedBytes = AesEncrypt(fileBytes, keyBytes);
                File.WriteAllBytes(_fileName, encryptedBytes);

                stopwatch.Stop();

                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Encryption failed for file '{_fileName}'", ex);
            }
        }

        /*
         * DecryptFile() reads the encrypted file and key
         * Decrypts the file using AesDecrypt()
         * Writes the decrypted file
         * Returns the decryption time in ms
        */
        public int DecryptFile()
        {
            if (!CheckFile()) return -1;

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var fileBytes = File.ReadAllBytes(_fileName);
                byte[] keyBytes = File.ReadAllBytes(_keyPath);
                var decryptedBytes = AesDecrypt(fileBytes, keyBytes);
                File.WriteAllBytes(_fileName, decryptedBytes);

                stopwatch.Stop();

                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Decryption failed for file '{_fileName}'", ex);
            }
        }
        
        /*
         * AesEncrypt() encrypts a file with a key using the AES method
         * Generates an IV so that the encrypted file is never the same
         * MemoryStream contains IV + ciphertext encrypted with CryptoStream
        */
        public byte[] AesEncrypt(byte[] fileBytes, byte[] keyBytes)
        {
            using (Aes aes = Aes.Create())
            using (var sha256 = SHA256.Create())
            {
                aes.Key = sha256.ComputeHash(keyBytes);
                aes.GenerateIV();

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(fileBytes, 0, fileBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        /*
         * AesDecrypt() decrypts an encrypted file with a key using the AES method
         * Recovers the IV which was used for encryption
         * Creates a CryptoStream on the ciphertext only (without IV)
        */
        public byte[] AesDecrypt(byte[] encryptedBytes, byte[] keyBytes)
        {
            using (Aes aes = Aes.Create())
            using (var sha256 = SHA256.Create())
            {
                aes.Key = sha256.ComputeHash(keyBytes);

                byte[] iv = new byte[16];
                Array.Copy(encryptedBytes, 0, iv, 0, 16);
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(
                        new MemoryStream(encryptedBytes, 16, encryptedBytes.Length - 16),
                        aes.CreateDecryptor(),
                        CryptoStreamMode.Read))
                    {
                        cs.CopyTo(ms);
                    }

                    return ms.ToArray();
                }
            }
        }
    }
}
