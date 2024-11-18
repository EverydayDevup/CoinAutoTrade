using System.Security.Cryptography;
using System.Text;

namespace AutoTrade.Logic;

public static class Crypto
{
    public static string Encrypt(string plainText, string password)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(password);
        aes.IV = Encoding.UTF8.GetBytes(password);

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
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(password);
        aes.IV = Encoding.UTF8.GetBytes(password);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        return reader.ReadToEnd();
    }
}