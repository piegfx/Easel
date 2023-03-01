using System.IO;

namespace Easel.Content.Builder;

public class ImageContent : IContentType
{
    public string Path;
    
    public ImageContent() { }

    public ImageContent(string path)
    {
        Path = path;
        FriendlyName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path));
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