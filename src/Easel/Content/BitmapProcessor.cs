using System.IO;
using Easel.Content.Builder;
using Easel.Graphics;

namespace Easel.Content;

public class BitmapProcessor : IContentProcessor
{
    public object Load(string contentDir, IContentType contentType)
    {
        ImageContent content = (ImageContent) contentType;
        return new Bitmap(Path.Combine(contentDir, content.Path));
    }
}