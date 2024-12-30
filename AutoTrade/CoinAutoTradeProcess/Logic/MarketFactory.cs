// using AutoTrade.Market;
// using Newtonsoft.Json;
// using SharedClass;
//
// namespace AutoTrade.Logic;
//
// public enum EMarketMenu
// {
//     Invalid,
//     Create,
//     Load,
//     Max
// }
//
// public enum EMarketType
// {
//     Invalid = 0,
//     Bithumb,
//     Max,
// }
//
//
// public static class MarketFactory
// {
//     /// <summary>
//     /// 마켓 생성 정보
//     /// </summary>
//     private static readonly Dictionary<EMarketType, Func<string, string, IMarket>> DicMarketFactory = new ()
//     {
//         {EMarketType.Bithumb, (accessKey, secretKey) => new Bithumb(accessKey, secretKey)}
//     };
//
//     /// <summary>
//     /// AES128 암복호화를 위한 키 값
//     /// </summary>
//     private const byte PasswordLength = 16;
//     
//     /// <summary>
//     /// 사용할 마켓 정보를 선택
//     /// </summary>
//     public static EMarketType SelectMarketType()
//     {
//         var marketType = EMarketType.Invalid;
//         
//         do
//         {
//             Console.WriteLine("Select market type Id");
//
//             foreach (EMarketType type in Enum.GetValues(typeof(EMarketType)))
//             {
//                 if (type == EMarketType.Invalid || type == EMarketType.Max)
//                     continue;
//                 
//                 Console.WriteLine($"{(int)type}.{type.ToString()}");
//             }
//             
//             Console.WriteLine("Please select market type Id : ");
//
//             var selectMarket = Console.ReadLine();
//
//             if (int.TryParse(selectMarket, out var select))
//             {
//                 if (select > (int)EMarketType.Invalid && select < (int)EMarketType.Max)
//                     marketType = (EMarketType)select;
//             }
//             else
//             {
//                 Console.WriteLine("Please select again.");
//             }
//
//         } while (marketType == EMarketType.Invalid);
//
//         return marketType;
//     }
//
//     /// <summary>
//     /// 사용할 마켓 메뉴를 선택
//     /// </summary>
//     /// <returns></returns>
//     public static EMarketMenu SelectMarketMenu()
//     {
//         var marketMenu = EMarketMenu.Invalid;
//         do
//         {
//             Console.WriteLine("Select market menu");
//
//             foreach (EMarketMenu type in Enum.GetValues(typeof(EMarketMenu)))
//             {
//                 if (type == EMarketMenu.Invalid || type == EMarketMenu.Max)
//                     continue;
//                 
//                 Console.WriteLine($"{(int)type}.{type.ToString()}");
//             }
//             
//             Console.WriteLine("Please select market menu id : ");
//
//             var selectMenu = Console.ReadLine();
//
//             if (int.TryParse(selectMenu, out var select))
//             {
//                 if (select > (int)EMarketMenu.Invalid && select < (int)EMarketMenu.Max)
//                     marketMenu = (EMarketMenu)select;
//             }
//             else
//             {
//                 Console.WriteLine("Please select again.");
//             }
//
//         } while (marketMenu == EMarketMenu.Invalid);
//
//         return marketMenu;
//     }
//
//     /// <summary>
//     /// 선택한 거래소에 따라 마켓 정보를 생성할지, 불러올지를 결정
//     /// </summary>
//     public static IMarket? GetMarket(EMarketType marketType, EMarketMenu marketMenu)
//     {
//         IMarket? market = null;
//         switch (marketMenu)
//         {
//             case EMarketMenu.Create:
//                 market = CreateMarketConfig(marketType);
//                 break;
//             case EMarketMenu.Load:
//                 market = LoadMarketConfig(marketType);
//                 break;
//         }
//
//         return market;
//     }
//
//     /// <summary>
//     /// 사용자의 Id 정보를 가져옴
//     /// </summary>
//     private static string GetId()
//     {
//         string? id;
//         do
//         {
//             Console.WriteLine("Please enter the id : ");
//             id = Console.ReadLine();
//             
//             if (string.IsNullOrEmpty(id))
//                 Console.WriteLine("Invalid id.");
//             
//         } while (string.IsNullOrEmpty(id));
//
//         return id;
//     }
//
//     /// <summary>
//     /// 사용자의 패스워드 정보를 가져옴
//     /// </summary>
//     /// <returns></returns>
//     private static string GetPassword()
//     {
//         string? password;
//         do
//         {
//             Console.WriteLine("Please enter your password : ");
//             password = Console.ReadLine();
//             
//             if (string.IsNullOrEmpty(password))
//                 Console.WriteLine("Invalid password.");
//             else if (password.Length != PasswordLength)
//             {
//                 password = string.Empty;
//                 Console.WriteLine($"password must be exactly {PasswordLength} characters.");
//             }
//             
//         } while (string.IsNullOrEmpty(password));
//
//         return password;
//     }
//     
//     /// <summary>
//     /// 사용자의 거래소 액세스 키를 가져옴
//     /// </summary>
//     private static string GetAccessKey()
//     {
//         string? accessKey;
//         do
//         {
//             Console.WriteLine("Please enter the accessKey : ");
//             accessKey = Console.ReadLine();
//             
//             if (string.IsNullOrEmpty(accessKey))
//                 Console.WriteLine("Invalid accessKey.");
//             
//         } while (string.IsNullOrEmpty(accessKey));
//
//         return accessKey;
//     }
//     
//     /// <summary>
//     /// 사용자의 거래소 비밀 키를 가져옴
//     /// </summary>
//     private static string GetSecretKey()
//     {
//         string? secretKey;
//         do
//         {
//             Console.WriteLine("Please enter the secretKey : ");
//             secretKey = Console.ReadLine();
//             
//             if (string.IsNullOrEmpty(secretKey))
//                 Console.WriteLine("Invalid secretKey.");
//             
//         } while (string.IsNullOrEmpty(secretKey));
//
//         return secretKey;
//     }
//
//     /// <summary>
//     /// 거래소 정보를 저장할 디렉토리
//     /// </summary>
//     private static string GetMarketConfigDirectoryPath()
//     {
//         var currentDirectory = Directory.GetCurrentDirectory();
//         var marketConfigDirectoryPath = Path.Combine(currentDirectory, PathConfig.MarketConfigDirectoryName);
//         
//         if (!Directory.Exists(marketConfigDirectoryPath))
//             Directory.CreateDirectory(marketConfigDirectoryPath);
//
//         return marketConfigDirectoryPath;
//     }
//
//     /// <summary>
//     /// 거래소 정보를 저장할 파일 경로
//     /// </summary>
//     private static string GetMarketConfigFilePath(EMarketType marketType, string id)
//     {
//         var directoryPath = GetMarketConfigDirectoryPath();
//         return Path.Combine(directoryPath, $"{marketType.ToString()}_{id}.json");
//     }
//     
//     /// <summary>
//     /// 거래소 정보를 생성
//     /// </summary>
//     private static IMarket? CreateMarketConfig(EMarketType marketType)
//     {
//         var id = GetId();
//         
//         var marketConfigFilePath = GetMarketConfigFilePath(marketType, id);
//         // 파일이 있다면 불러올 수 있도록, 불러오기 로직으로 넘김
//         if (File.Exists(marketConfigFilePath))
//         {
//             Console.WriteLine($"The file already exists : {marketConfigFilePath}");
//             return LoadMarketConfig(marketType);
//         }
//         
//         var accessKey = GetAccessKey();
//         var secretKey = GetSecretKey();
//         var password = GetPassword();
//
//         var marketConfig = new MarketConfig
//         {
//             AccessKey = accessKey,
//             SecretKey = secretKey
//         };
//         
//         try
//         {
//             var marketConfigJson = JsonConvert.SerializeObject(marketConfig);
//             var encryptText = Crypto.Encrypt(marketConfigJson, password);
//
//             File.WriteAllText(marketConfigFilePath, encryptText);
//
//             if (DicMarketFactory.TryGetValue(marketType, out var factory))
//                 return factory.Invoke(marketConfig.AccessKey, marketConfig.SecretKey);
//             
//             Console.WriteLine($"not found {nameof(marketType)}: {marketType.ToString()}");
//             return null;
//
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine(ex.Message);
//             return null;
//         }
//     }
//
//     /// <summary>
//     /// 마켓 정보를 불러옴
//     /// </summary>
//     private static IMarket? LoadMarketConfig(EMarketType marketType)
//     {
//         var id = GetId();
//         var marketConfigFilePath = GetMarketConfigFilePath(marketType, id);
//
//         if (!File.Exists(marketConfigFilePath))
//         {
//             Console.WriteLine($"file not found : {marketConfigFilePath}");
//             return null;
//         }
//         
//         var password = GetPassword();
//
//         try
//         {
//             var cipherText = File.ReadAllText(marketConfigFilePath);
//             var marketConfigJson = Crypto.Decrypt(cipherText, password);
//             var marketConfig = JsonConvert.DeserializeObject<MarketConfig>(marketConfigJson);
//             
//             if (marketConfig == null)
//             {
//                 Console.WriteLine($"file not found {nameof(marketConfig)}");
//                 return null;
//             }
//
//             if (DicMarketFactory.TryGetValue(marketType, out var factory))
//                 return factory.Invoke(marketConfig.AccessKey, marketConfig.SecretKey);
//             
//             Console.WriteLine($"not found {nameof(marketType)}: {marketType.ToString()}");
//             return null;
//
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine(ex.Message);
//             return null;
//         }
//     }
// }