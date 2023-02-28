using System.IO;
using Easel.Content.ContentFile;
using Easel.Formats;
using Easel.Graphics;

namespace Easel.Content;

public class TextureProcessor : IContentProcessor
{
    public object Load(string contentDir, IContentType contentType)
    {
        ImageContent content = (ImageContent) contentType;
        string extension = Path.GetExtension(content.Path);
        string fullPath = Path.Combine(contentDir, content.Path);
        switch (extension)
        {
            case ".dds":
                DDS dds = new DDS(File.ReadAllBytes(fullPath));
                return new Texture2D(dds.Bitmaps[0][0]);

            default:
                return new Texture2D(fullPath);
        }
    }
}