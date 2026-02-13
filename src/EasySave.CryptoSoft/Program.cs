
namespace EasySave.CryptoSoft
{
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1 || File.Exists(args[0]) == false)
            {
                Console.WriteLine("Usage: program <file>");
                return -1;
            }

            var fileManager = new FileManager(args[0]);
            try
            {
                int elapsedTime = fileManager.TransformFile();
                Console.WriteLine($"Temps : {elapsedTime} ms");
                return elapsedTime;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
