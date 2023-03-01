using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Easel.Core;

namespace Easel.Content.Builder;

public class ContentBuilder
{
    private string _directory;
    private string _name;
    private List<IContentType> _types;

    public ContentBuilder(string contentName)
    {
        _name = contentName;
        _directory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), contentName);
        _types = new List<IContentType>();
    }

    public ContentBuilder Add(IContentType type)
    {
        _types.Add(type);
        return this;
    }

    public ContentDefinition Build()
    {
        Logger.Debug("Building content...");
        
        Dictionary<string, IContentType> contentTypes = new Dictionary<string, IContentType>();

        foreach (IContentType type in _types)
        {
            string name = type.FriendlyName;
            Logger.Debug($"Building \"{name}\"...");
            if (contentTypes.ContainsKey(name))
                Logger.Fatal($"Duplicate content definition \"{name}\".");

            ContentValidity validity = type.CheckValidity(_directory);
            if (!validity.IsValid)
                Logger.Fatal($"Content not valid: {validity.Exception.Message}");
            
            contentTypes.Add(name, type);
        }

        return new ContentDefinition(_directory, contentTypes);
    }
}