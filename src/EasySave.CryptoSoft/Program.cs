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

    }
}
