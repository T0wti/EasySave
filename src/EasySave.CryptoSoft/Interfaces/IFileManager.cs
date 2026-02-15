using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.CryptoSoft.Interfaces
{
    public interface IFileManager
    {
        int EncryptFile();
        int DecryptFile();
    }
}
