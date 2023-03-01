namespace Easel.Content.Builder;

public interface IContentType
{
    public string FriendlyName { get; }
    
    public bool AllowDuplicates { get; }
    
    public ContentValidity CheckValidity(string contentPath);
}