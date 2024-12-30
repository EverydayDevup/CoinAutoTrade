﻿using Newtonsoft.Json;
using SharedClass;

namespace CoinAutoTrade;

public static class CoinTradeDataManager
{
    private static string GetAllCoinTradeDataFilePath(string id)
    {
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(CoinTradeData)}");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        
        return Path.Combine(directoryPath, $"{id}.json");
    }
    
    private static void SaveAllCoinTradeData(string id, List<CoinTradeData> allCoinTradeData)
    {
        var filePath = GetAllCoinTradeDataFilePath(id);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(allCoinTradeData));
    }

    public static List<CoinTradeData>? GetAllCoinTradeData(string id)
    {
        List<CoinTradeData>? allCoinTradeData = new();
        
        var filePath = GetAllCoinTradeDataFilePath(id);
        if (File.Exists(filePath))
            allCoinTradeData = JsonConvert.DeserializeObject<List<CoinTradeData>>(File.ReadAllText(filePath));

        return allCoinTradeData;
    }
    
    public static bool DeleteAllCoinTradeData(string id)
    {
        var filePath = GetAllCoinTradeDataFilePath(id);
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool AddCoinTradeData(string id, CoinTradeData coinTradeData)
    {
        try
        {
            var allCoinTradeData = GetAllCoinTradeData(id) ?? new();
            var find = allCoinTradeData.Find((data) => data.Symbol == coinTradeData.Symbol);
            if (find != null)
                allCoinTradeData.Remove(find);

            allCoinTradeData.Add(coinTradeData);
            SaveAllCoinTradeData(id, allCoinTradeData);

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    
    public static bool DeleteCoinTradeData(string id, string symbol)
    {
        try
        {
            var allCoinTradeData = GetAllCoinTradeData(id) ?? new();
            var find = allCoinTradeData.Find((data) => data.Symbol == symbol);
            if (find == null)
                return true;
            
            allCoinTradeData.Remove(find);
            SaveAllCoinTradeData(id, allCoinTradeData);

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static CoinTradeData? GetCoinTradeData(string id, string symbol)
    {
        var allCoinTradeData = GetAllCoinTradeData(id) ?? new();
        return allCoinTradeData.Find((data) => data.Symbol == symbol);
    }
}