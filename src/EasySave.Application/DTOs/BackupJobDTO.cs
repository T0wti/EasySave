namespace EasySave.Application.DTOs
{

    public class BackupJobDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string Type { get; set; }
    }
}
