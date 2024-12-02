using SharedClass;

namespace CoinAutoTradeClient;

public partial class CoinAutoTradeConsole
{
    private static int SelectMenu<T>(string menuMessage, List<T> menus) where T : Enum
    {
        var menu = -1;
        do
        {
            LoggerService.ConsoleLog(menuMessage);
            for (var i = 0; i < menus.Count; i++)
                LoggerService.ConsoleLog($"{i+1}.{menus[i].ToString()}");
            
            LoggerService.ConsoleLog("Please select menu id : ");

            var selectMenu = Console.ReadLine();
            if (int.TryParse(selectMenu, out var select))
            {
                select -= 1;
                if (select >= 0 && select < menus.Count)
                    menu = select;
            }
            else
            {
                menu = -1;
                LoggerService.ConsoleLog("Please select again.");
            }

        } while (menu == -1);

        return menu;
    }
    
    /// <summary>
    /// 사용자의 Id 정보를 가져옴
    /// </summary>
    private static string GetText(string message)
    {
        string? text;
        do
        {
            LoggerService.ConsoleLog(message);
            text = Console.ReadLine();
            
            if (string.IsNullOrEmpty(text))
                LoggerService.ConsoleLog($"Invalid {nameof(text)}");
            
        } while (string.IsNullOrEmpty(text));

        return text;
    }
    
    /// <summary>
    /// 사용자의 패스워드 정보를 가져옴
    /// </summary>
    /// <returns></returns>
    private static string GetPassword()
    {
        var password = string.Empty;
        var inputComplete = false;
        ConsoleKeyInfo keyInfo;
        
        LoggerService.ConsoleLog("Please enter your password : ");

        do
        {
            keyInfo = Console.ReadKey(intercept: true); // 화면에 표시하지 않음
            if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                // 백스페이스 처리
                password = password[..^1];
                Console.Write("\b \b"); // 화면에서 문자 제거
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                // 입력된 문자를 비밀번호에 추가
                password += keyInfo.KeyChar;
                Console.Write("*"); // 별표로 표시
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (string.IsNullOrEmpty(password))
                    LoggerService.ConsoleLog("Invalid password.");
                else if (password.Length != Crypto.PasswordLength)
                    LoggerService.ConsoleLog($"password must be exactly {Crypto.PasswordLength} characters.");
                else
                    inputComplete = true;
            }
            
        } while (!inputComplete);

        Console.WriteLine();
        return password;
    }
}