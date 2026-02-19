namespace EasySave.Application.Utils;

public static class CommandRunner
{
    // Parses CLI args and executes the matching backup jobs in parallel
    public static bool TryRun(string[] args, BackupAppService backupAppService)
    {
        if (args == null || args.Length == 0)
            return false;

        try
        {
            var ids = BackupIdParser.ParseIds(args[0]);
            // Block synchronously since TryRun is called from a sync Main
            backupAppService.ExecuteMultiple(ids).GetAwaiter().GetResult();
            Console.WriteLine("Backup execution finished.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return true;
    }
}