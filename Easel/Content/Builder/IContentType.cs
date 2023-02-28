using System;
using Easel.Core;

namespace Easel.Content.ContentFile;

public interface IContentType
{
    public string FriendlyName { get; }
    
    public bool AllowDuplicates { get; }
    
    public ContentValidity CheckValidity(string contentPath);
}