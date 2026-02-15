
namespace EasySave.CryptoSoft
{
    public class Program
    {
        // Program.cs — args[0] = file to encrypt, args[1] = key path, args[2] = usage option
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: CryptoSoft <filePath> <keyPath> <encrpyt|decrypt>");
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

            if (args[2] == "encrypt")
            {
                return fileManager.EncryptFile();
            } else if (args[2] == "decrypt")
            {
                return fileManager.DecryptFile();
            } else {
                Console.Error.WriteLine($"The third argument must be <encrypt> or <decrypt>: {args[2]}");
                return -1;
            }
        }
    }
}
