namespace SharedClass;

public class LoginResponse : ResponseBody
{
    /// <summary>
    /// 패킷 암복호화에 사용하는 키 값
    /// </summary>
    public string Key { get; init; } = string.Empty;
}