using EasySave.Application.Controllers;
using EasySave.Application.DTOs;
using EasySave.Console.Commands;
using EasySave.Console.ConsoleUI;
using EasySave.Console.Resources;
using EasySave.Domain.Enums;

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

        _texts = settings.Language == EasySave.Domain.Enums.Language.French
            ? new FrenchTextProvider()
            : new EnglishTextProvider();
    }

    public void RunConsole()
    {
        RunBaseMenu();
    }

    internal void RunBaseMenu()
    {
        var menu = new BaseMenu(_texts);
        var loop = new BaseMenuInteraction(this, _texts);
        menu.Display();
        loop.RunLoop();
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        _texts = language;

        var langEnum = language is FrenchTextProvider
            ? EasySave.Domain.Enums.Language.French
            : EasySave.Domain.Enums.Language.English;

        _configController.ChangeLanguage(langEnum);

        RunBaseMenu();
    }

    internal void RunCreateBackupMenu()
    {
        var menu = new CreateBackupMenu(_texts);
        var loop = new CreateBackupMenuInteraction(this, menu);
        menu.Display();
        loop.RunLoop();
    }

    internal void RunDeleteBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var listMenu = new ListBackupMenu(_texts, jobs);
        listMenu.Display();

        var menu = new DeleteBackupMenu(_texts);
        var loop = new DeleteBackupMenuInteraction(this, menu);
        loop.RunLoop();
    }

    internal void RunEditBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var listMenu = new ListBackupMenu(_texts, jobs);
        listMenu.Display();

        var editMenu = new EditBackupMenu(_texts);
        var loop = new EditBackupInteraction(this, editMenu, jobs);
        loop.RunLoop();
    }

    internal void RunChangeLanguageMenu()
    {
        var menu = new ChangeLanguageMenu(_texts);
        var loop = new ChangeLanguageMenuInteraction(this);
        menu.Display();
        loop.RunLoop();
    }

    internal void RunListBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var menu = new ListBackupMenu(_texts, jobs);
        var loop = new ListBackupMenuInteraction(this);
        menu.Display();
        loop.RunLoop();
    }

    internal void RunBackupDetailMenu(BackupJobDTO job)
    {
        var menu = new BackupDetailMenu(_texts, job);
        menu.Display();
        System.Console.ReadLine();
    }

    internal void RunExeBackupMenu()
    {
        var jobs = _backupController.GetAll();
        var menu = new ExecuteBackupMenu(_texts);
        var loop = new ExecuteBackupMenuInteraction(this, jobs);
        menu.Display();
        loop.RunLoop();
    }

    internal void WrongInput()
    {
        System.Console.WriteLine(_texts.WrongInput);
    }

    // ======================
    // Handlers
    // ======================

    internal void HandleCreateBackup(string name, string source, string target, int typeChoice)
    {
        try
        {
            _backupController.CreateBackup(name, source, target, typeChoice);
            System.Console.WriteLine(_texts.BackupCreated);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }

        RunBaseMenu();
    }

    public void HandleEditBackup(int id, string name, string source, string target, int typeChoice)
    {
        _backupController.EditBackup(id, name, source, target, typeChoice);
        RunEditBackupMenu();
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

        RunBackupDetailMenu(job);
    }

    internal void HandleDeleteBackup(int id)
    {
        try
        {
            _backupController.DeleteBackup(id);
            System.Console.WriteLine(_texts.BackupDeleted);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
        }

        RunBaseMenu();
    }

    internal void HandleExecuteBackup(int id)
    {
        _backupController.ExecuteBackup(id);
        RunBaseMenu();
    }

    internal void HandleExecuteMultiple(IEnumerable<int> ids)
    {
        _backupController.ExecuteMultiple(ids);
        RunBaseMenu();
    }
}
