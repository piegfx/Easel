using Easel.Graphics;

namespace Easel.Content;

public class MeshContentTypeReader : IContentTypeReader
{
    public object LoadContentItem(string path)
    {
        return null;
        //return Mesh.FromFile(path);
    }
}