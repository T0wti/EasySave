
namespace EasySave.CryptoSoft
{
    public class Program
    {
        // Program.cs — args[0] = file to encrypt, args[1] = path of the key
        static int Main(string[] args)
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
    }
}
