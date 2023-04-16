using System.IO;
using Easel.GUI;

namespace Easel.Content.Builder;

public struct FontContent : IContentType
{
    public string Path { get; set; }

    public FontOptions FontOptions;
    
    public FontContent() { }

    public FontContent(string path, FontOptions? options = null)
    {
        Path = path;
        FriendlyName = System.IO.Path
            .Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path))
            .Replace('\\', '/');
        FontOptions = options ?? new FontOptions();
    }

    public string FriendlyName { get; set; }

    public bool AllowDuplicates => false;

    public ContentValidity CheckValidity(string contentPath)
    {
        if (!File.Exists(System.IO.Path.Combine(contentPath, Path)))
            return new ContentValidity(false, new FileNotFoundException($"The file at {Path} was not found.", Path));

        return new ContentValidity(true, null);
    }
}