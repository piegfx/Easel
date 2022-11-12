using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Easel.Content.Localization;
using Easel.Graphics;
using Easel.Utilities;

namespace Easel.Content;

public class ContentManager
{
    public string ContentRootDir;

    public Locale Locale;

    private string _localeDir;
    private Dictionary<string, string> _loadedLocales;
    
    public string LocaleDir
    {
        get => _localeDir;
        set
        {
            _localeDir = value;
            Logging.Log("Loading locales from directory...");
            Dictionary<string, string> locales = new Dictionary<string, string>();
            string dir = Path.Combine(ContentRootDir, value);
            if (!Directory.Exists(dir))
                return;
            
            foreach (string file in Directory.GetFiles(dir))
            {
                Console.WriteLine(file);
                try
                {
                    Locale locale = XmlSerializer.Deserialize<Locale>(File.ReadAllText(file));
                    Logging.Log("Loaded \"" + locale.Id + "\".");
                    locales.Add(locale.Id, file);
                }
                catch (Exception)
                {
                    Logging.Warn("File was found, but was not a valid locale file. For speed reasons, you should place all locales in a separate directory.");
                }
            }

            _loadedLocales = locales;
        }
    }
    
    private Dictionary<Type, IContentTypeReader> _contentTypes;

    public ContentManager(string contentRootDir = "Content")
    {
        _contentTypes = new Dictionary<Type, IContentTypeReader>();
        AddNewTypeReader(typeof(Texture2D), new Texture2DContentTypeReader());
        AddNewTypeReader(typeof(Mesh[]), new MeshContentTypeReader());

        ContentRootDir = contentRootDir;

        _loadedLocales = new Dictionary<string, string>();
        LocaleDir = "Locales";
    }

    /// <summary>
    /// Load the given file into the given type. Note: While you do not have to provide an extension, it's recommended
    /// that you do, as there will be a slight speed penalty for not providing the extension.
    /// </summary>
    /// <param name="path">The relative path to the file.</param>
    /// <typeparam name="T">The type of the object you want to load.</typeparam>
    /// <returns>The loaded file.</returns>
    public T Load<T>(string path)
    {
        Logging.Log($"Loading content item \"{path}\"...");
        string fullPath = Path.Combine(ContentRootDir, path);
        if (!Path.HasExtension(fullPath))
        {
            IEnumerable<string> dirs = Directory.GetFiles(Path.GetDirectoryName(fullPath)).Where(s => Path.GetFileNameWithoutExtension(s) == Path.GetFileName(path));
            if (dirs.Count() > 1)
                Logging.Warn("Multiple files were found with the given name. Provide a file extension to load a specific file. The first file found will be loaded.");
            fullPath = dirs.First();
        }

        return (T) _contentTypes[typeof(T)].LoadContentItem(fullPath);
    }

    public void SetLocale(string id)
    {
        Locale = XmlSerializer.Deserialize<Locale>(File.ReadAllText(_loadedLocales[id]));
        Locale.Strings = new Dictionary<string, string>(Locale.XmlStrings.Length);
        foreach (Locale.XmlLocale locale in Locale.XmlStrings)
        {
            string value = locale.Value.Replace("\\n", "\n");
            Locale.Strings.Add(locale.Key, value);
        }
    }

    /// <summary>
    /// Add a custom type reader to load content.
    /// </summary>
    /// <param name="type">The type to attach this reader to.</param>
    /// <param name="reader">The reader itself.</param>
    public void AddNewTypeReader(Type type, IContentTypeReader reader)
    {
        _contentTypes.Add(type, reader);
    }
}