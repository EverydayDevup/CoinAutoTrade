using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AutoTrade.Logic;

public static class JwtHelper
{
    /// <summary>
    /// Json Web Token 생성
    /// 업비트 인증 헤더 생성에서 c# 코드 부분을 가져옴
    /// 출처 : https://docs.upbit.com/docs/create-authorization-request
    /// </summary>
    public static string GenerateToken(JwtPayload payload, string secretKey)
    {
        var keyBytes = Encoding.Default.GetBytes(secretKey);
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
        var header = new JwtHeader(credentials);
        var secToken = new JwtSecurityToken(header, payload);
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
        var authorizationToken = "Bearer " + jwtToken;

        return authorizationToken;
    }

    /// <summary>
    /// 쿼리에 대한 해시 값 생성
    /// 업비트 인증 헤더 생성에서 c# 코드 부분을 가져옴
    /// 출처 : https://docs.upbit.com/docs/create-authorization-request
    /// </summary>
    public static string GenerateQuery(Dictionary<string, string> parameters)
    {
        var builder = new StringBuilder();
        foreach (var (key, value) in parameters)
            builder.Append(key).Append("=").Append(value).Append("&");
        var query = builder.ToString().TrimEnd('&');
        return GenerateQuery(query);
    }

    public static string GenerateQuery(string query)
    {
        var sha512 = SHA512.Create();
        var queryHashByteArray = sha512.ComputeHash(Encoding.UTF8.GetBytes(query));
        return BitConverter.ToString(queryHashByteArray).Replace("-", "").ToLower();
    }
}