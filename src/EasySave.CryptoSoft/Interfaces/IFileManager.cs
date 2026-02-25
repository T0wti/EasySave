namespace EasySave.CryptoSoft.Interfaces
{
    public interface IFileManager
    {
        byte[] AesEncrypt(byte[] fileBytes, byte[] keyBytes);
    }
}
