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

    public ContentDefinition Build(DuplicateHandling duplicateHandling = DuplicateHandling.Error)
    {
        Logger.Debug("Building content...");
        
        Dictionary<string, IContentType> contentTypes = new Dictionary<string, IContentType>();

        foreach (IContentType type in _types)
        {
            string name = type.FriendlyName;
            Logger.Debug($"Building \"{name}\"...");
            if (contentTypes.ContainsKey(name))
            {
                if (duplicateHandling == DuplicateHandling.Error)
                    Logger.Fatal($"Duplicate content definition \"{name}\".");
                else if (duplicateHandling == DuplicateHandling.Ignore)
                {
                    Logger.Warn($"Duplicate content definition \"{name}\" found. This file will be ignored.");
                    continue;
                }
            }

            ContentValidity validity = type.CheckValidity(_directory);
            if (!validity.IsValid)
                Logger.Fatal($"Content not valid: {validity.Exception.Message}");
            
            contentTypes.Add(name, type);
        }
        
        Logger.Debug("Building done!");

        return new ContentDefinition(_name, contentTypes);
    }

    public static ContentBuilder FromDirectory(string directory)
    {
        Logger.Debug("Auto-generating a content builder...");
        
        ContentBuilder builder = new ContentBuilder(directory);
        string fullDir = builder._directory;

        foreach (string file in Directory.GetFiles(fullDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(builder._directory, file);

            string extension = Path.GetExtension(file);
            switch (extension.ToLower())
            {
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                case ".psd":
                case ".hdr":
                case ".pic":
                case ".pnm":
                case ".dds":
                    builder.Add(new ImageContent(relativePath));
                    break;
                
                case ".wav":
                case ".ogg":
                    builder.Add(new SoundContent(relativePath));
                    break;
                
                case ".gltf":
                case ".glb":
                case ".obj":
                case ".dae":
                case ".fbx":
                    builder.Add(new ModelContent(relativePath));
                    break;
                
                case ".ttf":
                case ".otf":
                    builder.Add(new FontContent(relativePath));
                    break;
                
                default:
                    Logger.Warn($"File \"{relativePath}\" is of type either not recognized or not supported. Ignoring...");
                    break;
            }
        }

        return builder;
    }
}