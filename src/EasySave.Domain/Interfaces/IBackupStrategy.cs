using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for a backup strategy, defining how to determine which files should be copied
    public interface IBackupStrategy
    {
        /// <summary>
        /// Compares the source and target directories and returns the list of files
        /// that need to be copied according to the strategys rules.
        /// </summary>
        /// <param name="sourceDir">Full path of the directory to back up.</param>
        /// <param name="targetDir">Full path of the destination directory.</param>
        /// <returns>
        /// A list of <see cref="FileDescriptor"/> objects representing the files
        /// to be copied. Returns an empty list if no files need copying.
        /// </returns>
        List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir);
    }
}