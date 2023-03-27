using System.IO;
using Easel.Content.Builder;
using Easel.Graphics;

namespace Easel.Content;

public class ModelProcessor : IContentProcessor
{
    public object Load(string contentDir, IContentType contentType)
    {
        ModelContent content = (ModelContent) contentType;

        return new Model(Path.Combine(contentDir, content.Path), content.FlipUvs);
    }
}