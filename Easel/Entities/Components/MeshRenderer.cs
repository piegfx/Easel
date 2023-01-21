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
    private MaterialMesh _mesh;
    private Renderable _renderable;

    public MeshRenderer(MaterialMesh mesh)
    {
        _mesh = mesh;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        _renderable = Renderable.CreateFromMesh(_mesh);
    }

    protected internal override void Draw()
    {
        base.Draw();
        Matrix4x4 world = Transform.TransformMatrix *
                          (Entity.Parent?.Transform.TransformMatrix ?? Matrix4x4.Identity);
        Graphics.Renderer.AddOpaque(_renderable, world);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _renderable.Dispose();
    }
}