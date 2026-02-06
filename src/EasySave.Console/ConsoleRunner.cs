using EasySave.Console.Resources;
using System.Collections.Generic;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;

namespace EasySave.Console;

public class ConsoleRunner
{
    private ITextProvider _texts = new EnglishTextProvider();

    private readonly IBackupManagerService _backupService;
    private readonly IConfigurationService _configService;

    public ConsoleRunner()
    {
        //to do: vérifier si surcharge nécessaire
    }

    public ConsoleRunner(IBackupManagerService backupService, IConfigurationService configurationService)
    {
        _backupService = backupService;
        _configService = configurationService;
    }

    public void RunConsole()
    {
        RunBaseMenu();
    }

    internal void RunBaseMenu()
    {
        var menu = new ConsoleUI.BaseMenu(_texts);
        var loop = new Commands.BaseMenuInteraction();
        menu.Display();
        loop.RunLoop();
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        _texts = language;

        var langEnum = (language is FrenchTextProvider) ? Language.French : Language.English;

        var settings = new ApplicationSettings(langEnum);

        _configService.SaveSettings(settings);

        RunBaseMenu();
    }

    internal void RunChangeLanguageMenu()
    {
        var menu = new ConsoleUI.ChangeLanguageMenu(_texts);
        var loop = new Commands.ChangeLanguageMenuInteraction();
        menu.Display();
        loop.RunLoop();
    }

    internal void RunExeBackupMenu()
    {
        var menu = new ConsoleUI.ExecuteBackupMenu(_texts);
        var loop = new Commands.ExecuteBackupMenuInteraction();
        menu.Display();
        loop.RunLoop();
    }

    internal void WrongInput()
    {
        System.Console.WriteLine(_texts.WrongInput);
    }
    // Methods to call the saves in the infrastrcture
    internal void HandleCreateBackup()
    {
        System.Console.WriteLine("Create backup chosen...");

        try
        {
            System.Console.Write("Job Name: ");
            string name = System.Console.ReadLine();

            System.Console.Write("Source Path: ");
            string source = System.Console.ReadLine();

            System.Console.Write("Target Path: ");
            string target = System.Console.ReadLine();

            System.Console.Write("Type 1 for Full or 2 for Differential: ");
            string typeChoice = System.Console.ReadLine();

            BackupType type = (typeChoice == "2") ? BackupType.Differential : BackupType.Full;

            var newJob = new BackupJob(0, name, source, target, type);

            _backupService.CreateBackupJob(newJob);

            System.Console.WriteLine("Job created succesfully");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
        }
    }

    internal void HandleDeleteBackup()
    {
        System.Console.WriteLine("Delete backup chosen...");

        System.Console.Write("Enter the name of the job to delete: ");
        string name = System.Console.ReadLine();

        _backupService.DeleteBackupJob(name);

        System.Console.WriteLine("Job deleted successfully");
    }

    internal void HandleEditBackup()
    {
        System.Console.WriteLine("Edit backup chosen...");

        var jobs = _backupService.GetBackupJobs();

        //to do: be able to handle backups, not just list

        if (jobs.Count > 0) { 
            foreach(var job in jobs)
            {
                System.Console.WriteLine($"Name: {job.Name}");
                System.Console.WriteLine($"Type: {job.Type}");
                System.Console.WriteLine($"Source: {job.SourcePath}");
                System.Console.WriteLine($"Target: {job.TargetPath}");
            }
        }
        else
        {
            System.Console.WriteLine("No jobs found");
        }
    }

}