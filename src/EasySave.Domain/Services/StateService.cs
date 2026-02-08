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

        public void Initialize(BackupProgress progress, List<FileDescriptor> files)
        {
            progress.TotalFiles = files.Count;
            progress.TotalSize = files.Sum(f => f.Size);
            progress.RemainingFiles = progress.TotalFiles;
            progress.RemainingSize = progress.TotalSize;
            progress.Progression = 0;
            progress.LastUpdate = DateTime.Now;

            Upsert(progress);
        }

        public void Update(BackupProgress progress, FileDescriptor file, string targetPath)
        {
            progress.RemainingFiles--;
            progress.RemainingSize -= file.Size;

            long copiedSize = progress.TotalSize - progress.RemainingSize;

            progress.Progression = progress.TotalSize == 0
                ? 100
                : Math.Round((double)copiedSize / progress.TotalSize * 100, 2);

            progress.CurrentSourceFile = file.FullPath;
            progress.CurrentTargetFile = targetPath;
            progress.LastUpdate = DateTime.Now;

            Upsert(progress);
        }

        public void Complete(int backupJobId)
        {
            FinalizeAndClean(backupJobId, BackupJobState.Completed);
        }

        public void Fail(int backupJobId)
        {
            UpdateStateOnly(backupJobId, BackupJobState.Failed);
        }


        private void FinalizeAndClean(int backupJobId, BackupJobState finalState)
        {
            var states = _fileStateService.ReadState();

            var existing = states.FirstOrDefault(
                s => s.BackupJobId == backupJobId);

            if (existing != null)
            {
                existing.State = finalState;
                existing.LastUpdate = DateTime.Now;

                // Nettoyage des infos runtime
                existing.TotalFiles = 0;
                existing.TotalSize = 0;
                existing.RemainingFiles = 0;
                existing.RemainingSize = 0;
                existing.Progression = 0;

                existing.CurrentSourceFile = null;
                existing.CurrentTargetFile = null;

                _fileStateService.WriteState(states);
            }
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

