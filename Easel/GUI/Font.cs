using System;
using Pie.Freetype;

namespace Easel.GUI;

public class Font : IDisposable
{
    public readonly Face Face;
    
    public Font(string path)
    {
        Face = FontHelper.FreeType.CreateFace(path, 0);
    }

    public void Dispose()
    {
        
    }
}