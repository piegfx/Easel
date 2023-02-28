using System.IO;
using Easel.Content.ContentFile;

namespace Easel.Content;

public class ImageContent : IContentType
{
    public string Path;
    
    public ImageContent(string path) : this(path, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path))) { }

    public ImageContent(string path, string customName)
    {
        Path = path;
        FriendlyName = customName;
    }

    public string FriendlyName { get; }
    
    public bool AllowDuplicates => false;

    public ContentValidity CheckValidity(string contentPath)
    {
        if (!File.Exists(System.IO.Path.Combine(contentPath, Path)))
            return new ContentValidity(false, new FileNotFoundException($"The file at {Path} was not found.", Path));

        return new ContentValidity(true, null);
    }
}