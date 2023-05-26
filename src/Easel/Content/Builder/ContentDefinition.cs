using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Easel.Content.Builder;

public class ContentDefinition
{
    public readonly string Name;

    public readonly Dictionary<string, IContentType> ContentTypes;

    internal ContentDefinition(string name, Dictionary<string, IContentType> contentTypes)
    {
        Name = name;
        ContentTypes = contentTypes;
    }

    public string SerializeToJson(Formatting formatting = Formatting.Indented)
    {
        List<IContentType> contentTypes = new List<IContentType>(ContentTypes.Count);
        
        foreach ((_, IContentType type) in ContentTypes)
            contentTypes.Add(type);

        SerializableContentDefinition serializable = new SerializableContentDefinition()
        {
            Name = Name,
            ContentTypes = contentTypes.ToArray()
        };

        return JsonConvert.SerializeObject(serializable, formatting, new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }

    public static ContentDefinition FromJson(string json)
    {
        SerializableContentDefinition serializable = JsonConvert.DeserializeObject<SerializableContentDefinition>(json, new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        Dictionary<string, IContentType> contentTypes =
            new Dictionary<string, IContentType>(serializable.ContentTypes.Length);

        foreach (IContentType type in serializable.ContentTypes)
            contentTypes.Add(type.FriendlyName, type);

        return new ContentDefinition(serializable.Name, contentTypes);
    }

    private class SerializableContentDefinition
    {
        public string Name;
        
        [JsonProperty("Content")]
        public IContentType[] ContentTypes;

        public SerializableContentDefinition() { }
    }
}