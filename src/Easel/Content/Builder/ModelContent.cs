using System.IO;

namespace Easel.Content.Builder;

public struct ModelContent : IContentType
{
    public string Path { get; set; }
    
    public bool FlipUvs;

    public ModelContent() { }

    public ModelContent(string path, bool flipUvs = true)
    {
        Path = path;
        FriendlyName = System.IO.Path
            .Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path))
            .Replace('\\', '/');
        FlipUvs = flipUvs;
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