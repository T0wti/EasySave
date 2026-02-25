using System.Diagnostics;
using System.Threading;

namespace EasySave.CryptoSoft
{
    // Program.cs — args[0] = file to encrypt, args[1] = path of the key
    public class Program
    {
        private const string MutexName = "EasySave_CryptoSoft_Mutex";

        static int Main(string[] args)
        {

            using var mutex = new Mutex(true, MutexName, out bool createdNew);// mutex disposed here by using : OS releases it at process exit

            if (!createdNew)
            {
                Console.Error.WriteLine("CryptoSoft is already running.");
                return -10; 
            }

            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: CryptoSoft <keyPath>");
                return -1;
            }

            if (!File.Exists(args[0]))
            {
                Console.Error.WriteLine($"Key file not found: {args[1]}");
                return -1;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();

                byte[] keyBytes = File.ReadAllBytes(args[0]);
                byte[] fileBytes;

                // Read the bytes on stdin
                using (var ms = new MemoryStream())
                {
                    Console.OpenStandardInput().CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                var fileManager = new FileManager(keyBytes);
                var encrypted = fileManager.AesEncrypt(fileBytes, keyBytes);

                // Write the crypted file on stdout
                Console.OpenStandardOutput().Write(encrypted, 0, encrypted.Length);

                stopwatch.Stop();
                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch
            {
                return -2;
            }
        }

    }
}
