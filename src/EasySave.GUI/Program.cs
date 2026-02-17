using Avalonia;
using EasySave.Application;
using EasySave.Application.Utils;
using System;

namespace EasySave.GUI;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Command mode with command runner
        var backupAppService = AppServiceFactory.CreateBackupController();
        if (CommandRunner.TryRun(args, backupAppService))
        {
            Environment.Exit(0);
            return;
        }

        // Mode GUI — launch the app
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}