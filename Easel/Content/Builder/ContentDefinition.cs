using System.Collections.Generic;

namespace Easel.Content.Builder;

public class ContentDefinition
{
    public string Name;

    public readonly Dictionary<string, IContentType> ContentTypes;

    internal ContentDefinition(string name, Dictionary<string, IContentType> contentTypes)
    {
        Name = name;
        ContentTypes = contentTypes;
    }
}