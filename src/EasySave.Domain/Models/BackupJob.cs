    using EasySave.Domain.Enums;

namespace EasySave.Domain.Models
{
    public class BackupJob
    {
        public int Id { get; }
        public string Name { get; internal set; }
        public string SourcePath { get; internal set; }
        public string TargetPath { get; internal set; }
        //public BackupType Type { get; internal set; }

        public BackupJob(
            int id,
            string name,
            string sourcePath,
            string targetPath//,
            //BackupType type
            )
        {
            Id = id;
            Name = name;
            SourcePath = sourcePath;
            TargetPath = targetPath;
            //Type = type;
        }
    }
}
