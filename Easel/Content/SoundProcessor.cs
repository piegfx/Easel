using System.IO;
using Easel.Audio;
using Easel.Content.Builder;

namespace Easel.Content;

public class SoundProcessor : IContentProcessor
{
    public object Load(string contentDir, IContentType contentType)
    {
        SoundContent content = (SoundContent) contentType;

        return new Sound(Path.Combine(contentDir, content.Path));
    }
}