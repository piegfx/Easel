using System.IO;

namespace Easel.Content.Builder;

public class ModelContent : IContentType
{
    public string Path;
    public bool FlipUvs;
    
    public ModelContent(string path, bool flipUvs = true) : this(path, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path)), flipUvs) { }

    public ModelContent(string path, string customName, bool flipUvs = true)
    {
        Path = path;
        FriendlyName = customName;
        FlipUvs = flipUvs;
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