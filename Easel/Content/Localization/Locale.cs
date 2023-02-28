using System.Collections.Generic;
using System.Xml.Serialization;
using XmlSerializer = Easel.Data.XmlSerializer;

namespace Easel.Content.Localization;

public class Locale
{
    public string Name;
    
    public Dictionary<string, string> Strings;

    public Locale()
    {
        Name = "Unknown";
        Strings = new Dictionary<string, string>();
    }
    
    public Locale(string name)
    {
        Name = name;

        Strings = new Dictionary<string, string>();
    }

    public string GetString(string key, params object[] format)
    {
        string text = Strings.TryGetValue(key, out string value)
            ? string.Format(value, format)
            : key;

        return text;
    }

    public string this[string key] => GetString(key, null);
}