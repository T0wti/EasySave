using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using EasySave.CryptoSoft.Interfaces;

namespace EasySave.CryptoSoft
{
    public class FileManager : IFileManager
    {
        private readonly string _fileName;
        //private readonly string _filePath;
        public FileManager(string fileName)
        {
            _fileName = fileName;
            //_filePath = filePath;
        }

        public bool CheckFile()
        {
            if (!File.Exists(_fileName)) return false;
            return true;
        }

        public int TransformFile()
        {
            if (!CheckFile()) return -1;

            //var filePath = Path.GetFullPath(_fileName);

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var fileBytes = File.ReadAllBytes(_fileName);
                //var keyBytes = Encoding.UTF8.GetBytes(_key);
                byte[] keyBytes = File.ReadAllBytes(@"adresse\key.txt");
                var encryptedBytes = AesEncrypt(fileBytes, keyBytes);
                File.WriteAllBytes(_fileName, encryptedBytes);

                //var output = _fileName + "";
                //File.WriteAllBytes(output, encryptedBytes);

                stopwatch.Stop();

                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -2;
            }
        }

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
