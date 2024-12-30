using System.Security.Cryptography;
using System.Text;
using Aes = System.Security.Cryptography.Aes;

namespace SharedClass;

public static class Crypto
{
    /// <summary>
    /// AES128 암복호화를 위한 키 값
    /// </summary>
    public const byte PasswordMaxLength = 16;
    public const byte PasswordMinLength = 8;
    
    /// <summary>
    /// 입력받은 패스워드길이와 관계없이 항상 16바이트 배열을 반환
    /// </summary>
    private static byte[] GetPassword(string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        return MD5.HashData(passwordBytes);
    }

    public static string GetSha256Hash(string plainText)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainText));
        var sb = new StringBuilder();
        foreach (var @byte in bytes)
            sb.Append(@byte.ToString("x2"));

        return sb.ToString();
    }

    /// <summary>
    /// AES로 평문을 암호화
    /// </summary>
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
    
    /// <summary>
    /// AES로 복호화
    /// </summary>
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