namespace YandexTranslateService.Cache;

public interface ICache
{
    string GetValue(string key, string sourceLang);
    void SetValue(string key, string sourceLang, string value);
    Dictionary<string, string> GetAllValues();
}