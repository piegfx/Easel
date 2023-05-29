using System;
using System.Numerics;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public interface IRenderer : IDisposable
{
    public RenderTarget2D MainTarget { get; }
    
    public bool InPass { get; }

    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in Vector3 cameraPosition, in SceneInfo sceneInfo);

    public void End3DPass();

    public void DrawRenderable(in Renderable renderable, in Matrix4x4 world);

    public void Resize(Size<int> newSize);
}