using EasySave.Application;
using EasySave.Application.Utils;

namespace EasySave.Console.Commands;

public static class ConsoleCommandRunner
{
    // Function to read option when launching the app on a terminal
    public static bool TryRun(string[] args, BackupAppService backupAppService)
    {
        if (args == null || args.Length == 0)
            return false;

        try
        {
            var ids = BackupIdParser.ParseIds(args[0]);

            backupAppService.ExecuteMultiple(ids);

            System.Console.WriteLine("Backup execution finished.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
        }

        return true;
    }
}
