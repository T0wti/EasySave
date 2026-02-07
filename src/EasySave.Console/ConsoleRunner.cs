using EasySave.Application.Controllers;
using EasySave.Console.Commands;
using EasySave.Console.ConsoleUI;
using EasySave.Console.Resources;
using EasySave.Domain.Enums;
using EasySave.Domain.Models;
using System.Xml.Linq;

namespace EasySave.Console;

public class ConsoleRunner
{
    private readonly BackupController _backupController;
    private readonly ConfigurationController _configController;

    private ITextProvider _texts;

    public ConsoleRunner(BackupController backupController, ConfigurationController configController)
    {
        _backupController = backupController;
        _configController = configController;

        var settings = _configController.Load();
        _texts = settings.Language == Language.French
            ? new FrenchTextProvider()
            : new EnglishTextProvider();
    }

    public void RunConsole()
    {
        RunBaseMenu();
    }

    internal void RunBaseMenu()
    {
        var menu = new ConsoleUI.BaseMenu(_texts);
        var loop = new Commands.BaseMenuInteraction(this, _texts);
        menu.Display();
        loop.RunLoop();
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        _texts = language;

        var langEnum = language is FrenchTextProvider
            ? Language.French
            : Language.English;

        _configController.ChangeLanguage(langEnum);

        RunBaseMenu();
    }
    internal void RunCreateBackupMenu()
    {
        var menu = new ConsoleUI.CreateBackupMenu(_texts);
        var loop = new Commands.CreateBackupInteraction(this, menu);
        menu.Display();
        loop.RunLoop();
    }

    internal void RunEditBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var listMenu = new ConsoleUI.ListBackupMenu(_texts, jobs);
        listMenu.Display();
        var editMenu = new ConsoleUI.EditBackupMenu(_texts);
        var loop = new Commands.EditBackupInteraction(this, editMenu, jobs);

        loop.RunLoop();
    }

    internal void RunChangeLanguageMenu()
    {
        var menu = new ConsoleUI.ChangeLanguageMenu(_texts);
        var loop = new Commands.ChangeLanguageMenuInteraction(this);
        menu.Display();
        loop.RunLoop();
    }

    internal void RunListBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var menu = new ConsoleUI.ListBackupMenu(_texts, jobs);
        var loop = new Commands.ListBackupMenuInteraction(this, menu);
        loop.RunLoop();
    }

    internal void RunBackupDetailMenu(BackupJob job)
    {
        var menu = new ConsoleUI.BackupDetailMenu(_texts, job);
        menu.Display();
        System.Console.ReadLine();
    }

    internal void RunExeBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var menu = new ConsoleUI.ExecuteBackupMenu(_texts);
        var loop = new Commands.ExecuteBackupMenuInteraction(this, jobs);
        menu.Display();
        loop.RunLoop();
    }

    internal void WrongInput()
    {
        System.Console.WriteLine(_texts.WrongInput);
    }

    // ======================
    // Backup handlers via controller
    // ======================

    internal void HandleCreateBackup(string name, string source, string target, int typeChoice  )
    {
        try
        {
            _backupController.CreateBackup(name, source, target, typeChoice);
            System.Console.WriteLine(_texts.BackupCreated);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
        RunBaseMenu();
    }

    // Dans ConsoleRunner.cs
    public void HandleEditBackup(int id, string name, string source, string target, int typeChoice)
    {
        var jobs = _backupController.GetAll();
        var listMenu = new ListBackupMenu(_texts, jobs);
        _backupController.EditBackup(id, name, source, target, typeChoice);

        listMenu.Display();

        var interaction = new EditBackupInteraction(this, new EditBackupMenu(_texts), jobs);
        interaction.RunLoop();
    }

    internal void HandleShowBackupDetail(int id)
    {
        if (id == 0)
        {
            RunListBackupMenu();
            return;
        }

        var job = _backupController.GetById(id);

        if (job == null)
        {
            WrongInput();
            return;
        }

        var menu = new ConsoleUI.BackupDetailMenu(_texts, job);
        menu.Display();
        System.Console.ReadLine(); 
    }


    internal void HandleDeleteBackup()
        {
        System.Console.WriteLine(_texts.EnterBackupToDelete); 
        if (int.TryParse(System.Console.ReadLine(), out int id))
        {
            _backupController.DeleteBackup(id);
            System.Console.WriteLine(_texts.BackupDeleted);
        }
    }

    internal void HandleEditBackup()
    {
        System.Console.WriteLine("Edit backup not implemented yet.");
    }

    internal void HandleExecuteBackup(int id)
    {
        _backupController.ExecuteBackup(id);
    }

    internal void HandleExecuteMultiple(IEnumerable<int> ids)
    {
        _backupController.ExecuteMultiple(ids);
    }
}
