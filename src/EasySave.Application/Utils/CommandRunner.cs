namespace EasySave.Application.Utils;

public static class CommandRunner
{
    // Parses CLI args and executes the matching backup jobs
    public static bool TryRun(string[] args, BackupAppService backupAppService)
    {
        if (args == null || args.Length == 0)
            return false;

        try
        {
            var ids = BackupIdParser.ParseIds(args[0]);
            backupAppService.ExecuteMultiple(ids);
            Console.WriteLine("Backup execution finished.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return true;
    }
}