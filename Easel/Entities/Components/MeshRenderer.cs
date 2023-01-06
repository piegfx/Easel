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
using Material = Easel.Graphics.Materials.Material;
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

        _renderables = Renderable.CreateFromMeshes(_meshes);
    }

    protected internal override void Draw()
    {
        base.Draw();

        for (int i = 0; i < _renderables.Length; i++)
        {
            ref Renderable renderable = ref _renderables[i];
            
            Matrix4x4 world = Transform.TransformMatrix *
                                      (Entity.Parent?.Transform.TransformMatrix ?? Matrix4x4.Identity);
            Graphics.Renderer.AddOpaque(renderable, world);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        
        for (int i = 0; i < _renderables.Length; i++)
            _renderables[i].Dispose();
    }
}