using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Easel.Primitives;
using Easel.Utilities;
using Pie;
using Pie.Utils;
using Silk.NET.Assimp;
using Material = Easel.Graphics.Material;
using Mesh = Easel.Utilities.Mesh;
using Texture = Pie.Texture;
using TextureType = Silk.NET.Assimp.TextureType;

namespace Easel.Entities.Components;

/// <summary>
/// The bog-standard 3D mesh renderer for an entity.
/// </summary>
public class MeshRenderer : Component
{
    private Mesh[] _meshes;
    private Renderable[] _renderables;
    
    private string _directory;
    private Dictionary<string, Texture2D> _loadedTextures;

    public MeshRenderer(Mesh[] meshes)
    {
        _meshes = meshes;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        GraphicsDevice device = Graphics.PieGraphics;
        
        _renderables = new Renderable[_meshes.Length];
        for (int i = 0; i < _meshes.Length; i++)
        {
            ref Mesh mesh = ref _meshes[i];
            _renderables[i] = Graphics.CreateRenderable(mesh);
        }
    }

    protected internal override void Draw()
    {
        base.Draw();

        for (int i = 0; i < _renderables.Length; i++)
        {
            ref Renderable renderable = ref _renderables[i];
            
            Matrix4x4 world = Transform.TransformMatrix *
                                      (Entity.Parent?.Transform.TransformMatrix ?? Matrix4x4.Identity);
            if (renderable.Material.Color.A < 1)
                Graphics.Renderer.DrawTranslucent(renderable, world);
            else
                Graphics.Renderer.DrawOpaque(renderable, world);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        
        for (int i = 0; i < _renderables.Length; i++)
            _renderables[i].Dispose();
    }
}