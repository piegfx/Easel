using System.Collections.Generic;
using System.IO;
using Easel.Content.Localization;

namespace Easel.Content;

public class LocaleContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        Locale locale = new Locale();
        locale.Strings = new Dictionary<string, string>();
        foreach (string line in File.ReadAllLines(path))
        {
            string ln = line.Trim();
            if (ln.StartsWith("#") || ln.StartsWith(";") || ln == "")
                continue;

            string[] splitLine = ln.Split('=');
            switch (splitLine[0].ToLower().Trim())
            {
                case "languageid":
                    locale.LanguageId = string.Join('=', splitLine[1..]).Trim();
                    break;
                case "languagename":
                    locale.LanguageName = string.Join('=', splitLine[1..]).Trim();
                    break;
                default:
                    locale.Strings.Add(splitLine[0].Trim(), string.Join('=', splitLine[1..]).Trim());
                    break;
            }
        }

        return locale;
    }
}