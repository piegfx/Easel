using Newtonsoft.Json;

namespace Easel.Content.Builder;

public interface IContentType
{
    public string Path { get; set; }
    
    public string FriendlyName { get; set; }
    
    [JsonIgnore]
    public bool AllowDuplicates { get; }
    
    public ContentValidity CheckValidity(string contentPath);
}