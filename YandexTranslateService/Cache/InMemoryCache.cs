using System.Collections.Concurrent;

namespace YandexTranslateService.Cache;

public class InMemoryCache : ICache
{
    private readonly ConcurrentDictionary<string, string> _cache = new();

    public string GetValue(string key, string sourceLang)
    {
        var compositeKey = $"{key}|{sourceLang}"; 
        _cache.TryGetValue(compositeKey, out var value);
        return value;
    }

    public void SetValue(string key, string sourceLang, string value)
    {
        var compositeKey = $"{key}|{sourceLang}"; 
        _cache[compositeKey] = value;
    }
    
    public Dictionary<string, string> GetAllValues()
    {
        var allValues = new Dictionary<string, string>();
        foreach (var kvp in _cache)
        {
            allValues[kvp.Key] = kvp.Value; 
        }
        return allValues;
    }
    
}