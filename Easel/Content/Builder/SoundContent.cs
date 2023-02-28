using System;
using System.IO;
using Easel.Content.ContentFile;

namespace Easel.Content.Builder;

public class SoundContent : IContentType
{
    public string Path;
    
    public SoundContent(string path) : this(path, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path))) { }

    public SoundContent(string path, string customName)
    {
        Path = path;
        FriendlyName = customName;
    }

    public string FriendlyName { get; }
    
    public bool AllowDuplicates => false;

    public ContentValidity CheckValidity(string contentPath)
    {
        string fullPath = System.IO.Path.Combine(contentPath, Path);
        if (!File.Exists(fullPath))
            return new ContentValidity(false, new FileNotFoundException($"The file at {Path} was not found.", Path));

        using FileStream fs = File.OpenRead(fullPath);
        using BinaryReader reader = new BinaryReader(fs);

        uint identifier = reader.ReadUInt32();
        if (identifier is 0x46464952 or 0x5367674F)
            return new ContentValidity(true, null);

        return new ContentValidity(false,
            new NotSupportedException(
                $"Unsupported sound type \"{System.IO.Path.GetExtension(Path)}\". Only .wav and .ogg supported."));
    }
}