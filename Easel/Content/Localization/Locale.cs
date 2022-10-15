using System.Collections.Generic;

namespace Easel.Content.Localization;

public class Locale
{
    public string LanguageId;

    public string LanguageName;

    public Dictionary<string, string> Strings;

    public string GetString(string key)
    {
        if (!Strings.TryGetValue(key, out string value))
            value = "?????";
        return value;
    }
}