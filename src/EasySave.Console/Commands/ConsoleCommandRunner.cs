using EasySave.Application.Controllers;

namespace EasySave.Console;

public static class ConsoleCommandRunner
{
    public static bool TryRun(string[] args, BackupController backupController)
    {
        if (args == null || args.Length == 0)
            return false; 

        try
        {
            var ids = ParseIds(args[0]);

            backupController.ExecuteMultiple(ids);

            System.Console.WriteLine("Backup execution finished.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
        }

        return true; 
    }

    private static IEnumerable<int> ParseIds(string arg)
    {
        arg = arg.Replace(" ", "");

        if (arg.Contains('-'))
        {
            var parts = arg.Split('-');

            int start = int.Parse(parts[0]);
            int end = int.Parse(parts[1]);

            if (start > end)
                throw new Exception("Invalid range");

            return Enumerable.Range(start, end - start + 1);
        }

        if (arg.Contains(';'))
        {
            return arg.Split(';')
                      .Select(int.Parse)
                      .Distinct();
        }

        return new[] { int.Parse(arg) };
    }
}
