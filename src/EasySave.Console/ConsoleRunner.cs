using EasySave.Application.Resources;
using EasySave.Console.Commands;
using EasySave.Console.ConsoleUI;
using EasySave.Console.Controllers;

namespace EasySave.Console
{
    // Responsible for navigation only — decides which menu to show
    // Only communicates with BackupController and ConfigController
    public class ConsoleRunner
    {
        private readonly BackupController _backupController;
        private readonly ConfigController _configController;
        private ITextProvider _texts;

        public ConsoleRunner(
            BackupController backupController,
            ConfigController configController,
            ITextProvider texts)
        {
            _backupController = backupController;
            _configController = configController;
            _texts = texts;
        }

        // --- Entry point ---

        public void Run()
        {
            if (!_configController.FileExists())
            {
                _configController.EnsureConfigExists();
                _configController.EnsureKeyExists();
                RunFirstStartMenu();
                return;
            }

            _configController.EnsureKeyExists();
            RunBaseMenu();
        }

        // --- Navigation ---

        internal void RunBaseMenu()
        {
            var menu = new BaseMenu(_texts);
            menu.Display();
            new BaseMenuInteraction(this, _texts).RunLoop();
        }

        internal void RunCreateBackupMenu()
        {
            var menu = new CreateBackupMenu(_texts);
            menu.Display();
            new CreateBackupMenuInteraction(this, menu, _backupController).RunLoop();
        }

        internal void RunListBackupMenu()
        {
            var jobs = _backupController.GetAll();
            var menu = new ListBackupMenu(_texts, jobs);
            menu.Display();
            new ListBackupMenuInteraction(this, menu).RunLoop();
        }

        internal void RunDeleteBackupMenu()
        {
            var jobs = _backupController.GetAll();
            new ListBackupMenu(_texts, jobs).Display();
            var menu = new DeleteBackupMenu(_texts);
            new DeleteBackupMenuInteraction(this, menu, _backupController).RunLoop();
        }

        internal void RunEditBackupMenu()
        {
            var jobs = _backupController.GetAll();
            new ListBackupMenu(_texts, jobs).Display();
            var menu = new EditBackupMenu(_texts);
            new EditBackupInteraction(this, menu, jobs, _backupController).RunLoop();
        }

        internal void RunExecuteBackupMenu()
        {
            var jobs = _backupController.GetAll();
            var menu = new ExecuteBackupMenu(_texts, jobs);
            menu.Display();
            new ExecuteBackupMenuInteraction(this, jobs, _backupController).RunLoop();
        }

        internal void RunChangeLanguageMenu()
        {
            var menu = new ChangeLanguageMenu(_texts);
            menu.Display();
            new ChangeLanguageMenuInteraction(this, _configController).RunLoop();
        }

        internal void RunChangeLogFormatMenu()
        {
            var menu = new ChangeLogFormatMenu(_texts, _configController);
            menu.Display();
            new ChangeLogFormatMenuInteraction(this, _configController).RunLoop();
        }

        internal void RunBackupDetailMenu(int id)
        {
            var job = _backupController.GetById(id);
            if (job == null)
            {
                WrongInput();
                return;
            }

            new BackupDetailMenu(_texts, job).Display();
            System.Console.ReadLine();
            RunListBackupMenu();
        }

        internal void RunFirstStartMenu()
        {
            var menu = new FirstStartMenu(_texts);
            menu.Display();
            new FirstStartMenuInteraction(_texts, this, _configController).FirstStartLoop();
        }

        internal void ChangeLanguage(ITextProvider newTexts, int code)
        {
            _configController.HandleChangeLanguage(code);
            _texts = newTexts;
            RunBaseMenu();
        }

        // --- Helpers ---

        internal void WrongInput()
        {
            System.Console.WriteLine(_texts.WrongInput);
        }
    }
}