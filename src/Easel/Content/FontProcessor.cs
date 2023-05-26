using System.IO;
using Easel.Content.Builder;
using Easel.GUI;

namespace Easel.Content;

public class FontProcessor : IContentProcessor
{
    public object Load(string contentDir, IContentType contentType)
    {
        FontContent content = (FontContent) contentType;
        return new Font(Path.Combine(contentDir, content.Path), content.FontOptions);
    }
}