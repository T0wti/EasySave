using EasySave.Application;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;
using EasySave.Application.Utils;

namespace EasySave.Console.Controllers
{
    // Handles all backup-related actions
    public class BackupController
    {
        private readonly BackupAppService _backupAppService;
        private readonly ITextProvider _texts;

        public BackupController(BackupAppService backupAppService, ITextProvider texts)
        {
            _backupAppService = backupAppService;
            _texts = texts;
        }

        public IEnumerable<BackupJobDTO> GetAll()
        {
            return _backupAppService.GetAll();
        }

        public BackupJobDTO? GetById(int id)
        {
            return _backupAppService.GetById(id);
        }

        public void HandleCreateBackup(string name, string source, string target, int typeChoice)
        {
            try
            {
                _backupAppService.CreateBackup(name, source, target, typeChoice);
                System.Console.WriteLine(_texts.BackupCreated);
            }
            catch 
            {
            }
        }

        public void HandleEditBackup(int id, string name, string source, string target, int typeChoice)
        {
            try
            {
                _backupAppService.EditBackup(id, name, source, target, typeChoice);
                System.Console.WriteLine(_texts.BackupEdited);
            }
            catch
            {
            }
        }

        public void HandleDeleteBackup(int id)
        {
            try
            {
                _backupAppService.DeleteBackup(id);
                System.Console.WriteLine(_texts.BackupDeleted);
            }
            catch
            {
            }
        }

        public void HandleExecuteBackup(int id)
        {
            try
            {
                _backupAppService.ExecuteBackup(id);
            }
            catch 
            {
            }
        }

        public void HandleExecuteMultiple(string arg)
        {
            try
            {
                var ids = BackupIdParser.ParseIds(arg);
                _backupAppService.ExecuteMultiple(ids);
            }
            catch 
            {
            }
        }
    }
}   