using System.IO;
using Easel.Formats;

namespace Easel.Content;

public class DDSContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return new DDS(File.ReadAllBytes(path));
    }
}