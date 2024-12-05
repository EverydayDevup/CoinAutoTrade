using System.Security.Cryptography;
using System.Text;

namespace SharedClass;

public static class Crypto
{
    /// <summary>
    /// AES128 암복호화를 위한 키 값
    /// </summary>
    public const byte PasswordMaxLength = 16;
    public const byte PasswordMinLength = 8;
    
    private static byte[] GetPassword(string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        return MD5.HashData(passwordBytes);
    }

    public static string Encrypt(string plainText, string password)
    {
        using var aes = Aes.Create();
        aes.Key = GetPassword(password);
        aes.IV = aes.Key;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }
        
        return Convert.ToBase64String(ms.ToArray());
    }
    
    public static string Decrypt(string cipherText, string password)
    {
        using var aes = Aes.Create();
        aes.Key = GetPassword(password);
        aes.IV = aes.Key;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        return reader.ReadToEnd();
    }
}