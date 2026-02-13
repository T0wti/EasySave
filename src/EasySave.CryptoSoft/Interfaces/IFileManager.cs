using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.CryptoSoft.Interfaces
{
    public interface IFileManager
    {
        int TransformFile();
        byte[] AesEncrypt(byte[] fileBytes, byte[] keyBytes);
        byte[] AesDecrypt(byte[] encryptedBytes, byte[] keyBytes);
    }
}
