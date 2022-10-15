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
    
    private Dictionary<Type, IContentTypeReader> _contentTypes;

    public ContentManager(string contentRootDir = "Content")
    {
        _contentTypes = new Dictionary<Type, IContentTypeReader>();
        AddNewTypeReader(typeof(Texture2D), new Texture2DContentTypeReader());
        AddNewTypeReader(typeof(Mesh[]), new MeshContentTypeReader());
        AddNewTypeReader(typeof(Locale), new LocaleContentTypeReader());
        
        ContentRootDir = contentRootDir;
    }

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

    public void AddNewTypeReader(Type type, IContentTypeReader reader)
    {
        _contentTypes.Add(type, reader);
    }
}