using System;
using Pie;

namespace Easel.Graphics.Renderers;

public class SpriteRenderer : IDisposable
{
    public const uint MaxSprites = 1 << 14;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxSprites;
    private const uint MaxIndices = NumIndices * MaxSprites;
    
    public SpriteRenderer(GraphicsDevice device)
    
    public void Dispose()
    {
        
    }
}