using System.Threading;

namespace EasySave.CryptoSoft
{
    // Program.cs — args[0] = file to encrypt, args[1] = path of the key
    public class Program
    {
        private static Mutex? _mutex;

        static int Main(string[] args)
        {
            bool createdNew;
            _mutex = new Mutex(true, "EasySave_CryptoSoft_Mutex", out createdNew);
            
            if (!createdNew)
            {
                Console.Error.WriteLine("CryptoSoft is already running.");
                return -10;
            }

            try
            {
                if (args.Length != 2)
                {
                    Console.Error.WriteLine("Usage: CryptoSoft <filePath> <keyPath>");
                    return -1;
                }

                if (!File.Exists(args[0]))
                {
                    Console.Error.WriteLine($"File not found: {args[0]}");
                    return -1;
                }

                if (!File.Exists(args[1]))
                {
                    Console.Error.WriteLine($"Key file not found: {args[1]}");
                    return -1;
                }

                var fileManager = new FileManager(args[0], args[1]);
                return fileManager.TransformFile();
            }
            finally // Release the mutex
            {
                _mutex.ReleaseMutex(); 
            }
        }
    }
}
