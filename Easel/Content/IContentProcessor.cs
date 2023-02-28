using Easel.Content.ContentFile;

namespace Easel.Content;

public interface IContentProcessor
{
    public object Load(string contentDir, IContentType contentType);
}