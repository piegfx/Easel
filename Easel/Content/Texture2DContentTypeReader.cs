using Easel.Graphics;

namespace Easel.Content;

public class Texture2DContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        // Don't dispose this texture as we'll leave the content manager's garbage collector to do this for us instead.
        // TODO: Figure out a potentially better solution.
        return new Texture2D(path, autoDispose: false);
    }
}