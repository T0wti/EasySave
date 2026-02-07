using EasySave.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using EasySave.Domain.Interfaces;
using Microsoft.Win32;

namespace EasySave.Domain.Services
{
    internal class PathService : IPathService
    {
        public string GetUncPath(string path)
        {
            //Get absolute path
            string fullPath = Path.GetFullPath(path);

            if (fullPath.StartsWith(@"\\")) { return fullPath; } //Already unc path like //server/...
            
            string pathRoot = Path.GetPathRoot(fullPath); //Get drive letter like Z:\
            string driveLetter = pathRoot.Substring(0, 1);

            //Read in registery key
            //HKEY_CURRENT_USER\Network\{letter} has the IP or FQDN of letter
            RegistryKey key = Registry.CurrentUser.OpenSubKey($@"Network\{driveLetter}");

            if (key != null)
            {
                string remotePath = key.GetValue("RemotePath").ToString();

                key.Close();

                if(!string.IsNullOrEmpty(remotePath))
                {
                    //Returns server\share\....
                    return fullPath.Replace(pathRoot, (remotePath + Path.DirectorySeparatorChar));
                }
            }

            return fullPath;
        }
    }
}
