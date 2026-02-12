    using EasySave.Application.Controllers;
    using EasySave.Console.Commands;
    using EasySave.Console.ConsoleUI;
    using EasySave.Application.DTOs;
    using EasySave.Application.Resources;

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
           
            // Check if it's the first launch of the program
            if (!_configController.FileExists())
            {
                _texts = new EnglishTextProvider();
                _configController.EnsureConfigExists();
                RunFirstStartMenu();
            }
            
            var settings = _configController.Load();

            _texts = settings.LanguageCode == 0
            ? new FrenchTextProviderConsole()
            : new EnglishTextProviderConsole();
        }
        // Main console execution
        public void RunConsole()
        {
            RunBaseMenu();
        }
        
        // Main menu
        internal void RunBaseMenu()
        {
            var menu = new ConsoleUI.BaseMenu(_texts);
            var loop = new Commands.BaseMenuInteraction(this, _texts);
            menu.Display();
            loop.RunLoop();
        }
        // Menu to change the language
        internal void ChangeLanguage(ITextProvider language)
        {
            _texts = language;

            int code = language is FrenchTextProvider ? 0 : 1;

            _configController.ChangeLanguage(code);

            RunBaseMenu();
        }

        // Menu to create a backup
        internal void RunCreateBackupMenu() 
        {
            var menu = new ConsoleUI.CreateBackupMenu(_texts);
            var loop = new Commands.CreateBackupMenuInteraction(this, menu);
            menu.Display();
            loop.RunLoop();
        }
            
        // Menu to delete a backup
        internal void RunDeleteBackupMenu() 
        {
            var jobs = _backupController.GetAll();
            var listMenu = new ConsoleUI.ListBackupMenu(_texts, jobs);
            listMenu.Display();
            var menu = new ConsoleUI.DeleteBackupMenu(_texts);
            var loop = new Commands.DeleteBackupMenuInteraction(this, menu);
            loop.RunLoop();
        }

        // Menu to edit a backup
        internal void RunEditBackupMenu()
        {
            var jobs = _backupController.GetAll();
            var listMenu = new ConsoleUI.ListBackupMenu(_texts, jobs);
            listMenu.Display();
            var editMenu = new ConsoleUI.EditBackupMenu(_texts);
            var loop = new Commands.EditBackupInteraction(this, editMenu, jobs);

            loop.RunLoop();
        }
        
        // Menu to change the language
        internal void RunChangeLanguageMenu()
        {
            var menu = new ConsoleUI.ChangeLanguageMenu(_texts);
            var loop = new Commands.ChangeLanguageMenuInteraction(this);
            menu.Display();
            loop.RunLoop();
        }

        // Menu to list all the backups
        internal void RunListBackupMenu()
        {
            var jobs = _backupController.GetAll();
            var menu = new ConsoleUI.ListBackupMenu(_texts, jobs); 
            var loop = new Commands.ListBackupMenuInteraction(this, menu);
            menu.Display();
            loop.RunLoop();
        }

        // Menu to execute backups
        internal void RunExeBackupMenu()
        {
            var jobs = _backupController.GetAll();
            var menu = new ConsoleUI.ExecuteBackupMenu(_texts,jobs);
            var loop = new Commands.ExecuteBackupMenuInteraction(this, jobs);
            menu.Display();
            loop.RunLoop();
        }

        // Menu to display execution status
        private void RunExecuteBackupMenuDetail(int i)
        {
            var menu = new ExecuteBackupDetail(_texts);
            menu.Display(i);
        }

        // Menu of the first start
        private void RunFirstStartMenu()
        {
            var menu = new FirstStartMenu(_texts);
            var loop = new FirstStartMenuInteraction(_texts, this);
            menu.Display();
            loop.FirstStartLoop();
        }

    internal void RunChangeLogFormatMenu()
    {
        var menu = new ConsoleUI.ChangeLogFormatMenu(_texts,_configController);
        var loop = new Commands.ChangeLogFormatMenuInteraction(this);
        menu.Display();
        loop.RunLoop();
    }

    // Function to show an error in the user input
    internal void WrongInput()
        {
            System.Console.WriteLine(_texts.WrongInput);
        }

        // ======================
        // Backup handlers via controller to link the front with the back
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

        // To execute multiple backups at once
        internal void HandleExecuteMultiple(IEnumerable<int> ids)
        {
            RunExecuteBackupMenuDetail(0);
            _backupController.ExecuteMultiple(ids);
            RunExecuteBackupMenuDetail(1);
            RunBaseMenu();
        }
    internal void HandleChangeLogFormat(int formatCode)
    {
     
            _configController.ChangeLogFormat(formatCode);
            System.Console.WriteLine(_texts.LogFormatChanged);
            System.Console.WriteLine();
            System.Console.ReadLine();
    
        RunBaseMenu();
    }

}
