using System;
using System.Collections.Generic;
using System.Text;
using System.IO; //For Directory, FileInfo, Path
using System.Linq; //For .Sum()

namespace EasySave.Infrastructure
{
    internal class CopyFile
    {
        private void CreateDirectory(string destDirectory)
        {
            if (!Directory.Exists(destDirectory)) //check if directory already exists
            {
                Directory.CreateDirectory(destDirectory);
            }
        }

        public FileInfo[] GetFiles(string sourceDirectory)
        {
            var dirInfo = new DirectoryInfo(sourceDirectory);
            return dirInfo.GetFiles();
        }
        public long GetTotalSize(FileInfo[] files)
        {
            //Sum() adds the Length property of each file
            return files.Sum(f => f.Length);
        }

        public void CopyFiles(FileInfo[] files, string destDirectory, Action<int> onProgress)
        {
            //Ensure destination exists before copying
            CreateDirectory(destDirectory);

            long totalSize = GetTotalSize(files);
            long totalCopied = 0;

            foreach (FileInfo file in files)
            {
                //File.Copy needs full path
                string destPath = Path.Combine(destDirectory, file.Name);

                file.CopyTo(destPath, true); //true to overwrite files

                //Update total copied size
                totalCopied += file.Length;

                int progress = 0;
                if (totalSize > 0) { //To avoid division by 0
                //Calculate percentage (cast to double to avoid integer division)
                    progress = (int)((double)totalCopied / totalSize * 100);
                }

                //Notify the progress (if a listener is attached)
                if (onProgress != null)
                {
                    onProgress(progress);
                }
            }
        }
    }
}