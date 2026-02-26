using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{

    public interface IFileService
    {
        /// <summary>
        /// Recursively enumerates all files in the given directory
        /// and returns their metadata as a flat list of descriptors.
        /// </summary>
        /// <param name="sourceDirectory">Full path of the directory to scan.</param>
        /// <returns>
        /// A list of <see cref="FileDescriptor"/> objects containing the path,
        /// size, and last-modified date of each file found.
        /// Returns an empty list if the directory is empty.
        /// </returns>
        List<FileDescriptor> GetFiles(string sourceDirectory);

        /// <summary>
        /// Copies a single file from the source path to the target path,
        /// creating any missing intermediate directories in the target.
        /// Overwrites the target file if it already exists.
        /// </summary>
        /// <param name="sourcePath">Full path of the file to copy.</param>
        /// <param name="targetPath">Full path of the destination file.</param>
        void CopyFile(string sourcePath, string targetPath);
    }
}