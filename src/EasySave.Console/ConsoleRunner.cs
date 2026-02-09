    using EasySave.Application.Controllers;
    using EasySave.Console.Commands;
    using EasySave.Console.ConsoleUI;
    using EasySave.Console.Resources;
    using EasySave.Application.DTOs;

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
            
            if (!_configController.FileExists())
            {
                _texts = new EnglishTextProvider();
                _configController.EnsureConfigExists();
                RunFirstStartMenu();
            }
            
            var settings = _configController.Load();

            _texts = settings.LanguageCode == 0
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

        int code = language is FrenchTextProvider ? 0 : 1;

        _configController.ChangeLanguage(code);

        RunBaseMenu();
    }

    internal void RunCreateBackupMenu()
        {
            var menu = new ConsoleUI.CreateBackupMenu(_texts);
            var loop = new Commands.CreateBackupMenuInteraction(this, menu);
            menu.Display();
            loop.RunLoop();
        }

        internal void RunDeleteBackupMenu()
        {
        var jobs = _backupController.GetAll();
        var listMenu = new ConsoleUI.ListBackupMenu(_texts, jobs);
        listMenu.Display();
        var menu = new ConsoleUI.DeleteBackupMenu(_texts);
        var loop = new Commands.DeleteBackupMenuInteraction(this, menu);
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
        menu.Display();
        loop.RunLoop();
    }


    internal void RunBackupDetailMenu(BackupJobDTO job)
    {
        var menu = new ConsoleUI.BackupDetailMenu(_texts, job);
        menu.Display();
    }

        internal void RunExeBackupMenu()
        {
            var jobs = _backupController.GetAll();
            var menu = new ConsoleUI.ExecuteBackupMenu(_texts,jobs);
            var loop = new Commands.ExecuteBackupMenuInteraction(this, jobs);
            menu.Display();
            loop.RunLoop();
        }

        private void RunFirstStartMenu()
        {
            var menu = new FirstStartMenu(_texts);
            var loop = new FirstStartMenuInteraction(_texts, this);
            menu.Display();
            loop.FirstStartLoop();
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

        public void HandleEditBackup(int id, string name, string source, string target, int typeChoice)
        {
            var jobs = _backupController.GetAll();
            var listMenu = new ListBackupMenu(_texts, jobs);
            _backupController.EditBackup(id, name, source, target, typeChoice);

            listMenu.Display();

            var menu = new EditBackupInteraction(this, new EditBackupMenu(_texts), jobs);

            menu.RunLoop();
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


        internal void HandleDeleteBackup(int id)
            {
        try
        {
            _backupController.DeleteBackup(id);
            System.Console.WriteLine(_texts.BackupDeleted);
        }
        catch (System.Exception ex)
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
        }
    }
