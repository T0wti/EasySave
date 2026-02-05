using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{
    public class StateService : IStateService
    {
        private readonly IFileStateService _fileStateService;

        public StateService(IFileStateService fileStateService)
        {
            _fileStateService = fileStateService;
        }

        public void Initialize(BackupProgress progress)
        {
            Upsert(progress);
        }

        public void Update(BackupProgress progress)
        {
            progress.LastUpdate = DateTime.Now;
            Upsert(progress);
        }

        public void Complete(int backupJobId)
        {
            UpdateStateOnly(backupJobId, BackupJobState.Completed);
        }

        public void Fail(int backupJobId)
        {
            UpdateStateOnly(backupJobId, BackupJobState.Failed);
        }

        private void Upsert(BackupProgress progress)
        {
            var states = _fileStateService.ReadState();

            var existing = states.FirstOrDefault(
                s => s.BackupJobId == progress.BackupJobId);

            if (existing != null)
                states.Remove(existing);

            states.Add(progress);
            _fileStateService.WriteState(states);
        }

        private void UpdateStateOnly(int backupJobId, BackupJobState state)
        {
            var states = _fileStateService.ReadState();

            var existing = states.FirstOrDefault(
                s => s.BackupJobId == backupJobId);

            if (existing != null)
            {
                existing.State = state;
                existing.LastUpdate = DateTime.Now;
                _fileStateService.WriteState(states);
            }
        }
    }
}