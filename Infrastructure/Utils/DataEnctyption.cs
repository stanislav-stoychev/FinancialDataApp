using NETCore.Encrypt;

namespace Utils;

public static class DataEncryption
{
    public static string EncryptString(string plainText, string key)
    {
        return EncryptProvider.AESEncrypt(plainText, key);
    }

    public static string DecryptString(string encryptedString, string key)
    {
        return EncryptProvider.AESDecrypt(encryptedString, key);
    }
}