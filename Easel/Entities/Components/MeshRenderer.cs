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

    /// <summary>
    /// Create a new <see cref="MeshRenderer"/> instance with the given <see cref="IPrimitive"/> and <see cref="Material"/>
    /// </summary>
    /// <param name="primitive">The <see cref="IPrimitive"/> mesh.</param>
    /// <param name="material">The <see cref="Material"/> to apply to the mesh.</param>
    public MeshRenderer(IPrimitive primitive, Material material)
    {
        _meshes = new Mesh[1];
        _meshes[0] = new Mesh(primitive.Vertices, primitive.Indices, material);
    }

    /// <summary>
    /// Create a new <see cref="MeshRenderer"/> instance from the given path.
    /// </summary>
    /// <param name="path">The path to load.</param>
    /// <param name="flipUvs">Some models load with incorrect UVs, this will flip them so they are correct.</param>
    public MeshRenderer(string path, bool flipUvs = false)
    {
        _meshes = Mesh.LoadFromFile(path, flipUvs);
    }

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
            _renderables[i] = new Renderable(device.CreateBuffer(BufferType.VertexBuffer, mesh.Vertices),
                device.CreateBuffer(BufferType.IndexBuffer, mesh.Indices), (uint) mesh.Indices.Length,
                Matrix4x4.Identity, mesh.Material, Graphics.EffectManager.GetEffectLayout(EffectManager.Forward.Diffuse));
        }
    }

    protected internal override void Draw()
    {
        base.Draw();

        for (int i = 0; i < _renderables.Length; i++)
        {
            ref Renderable renderable = ref _renderables[i];
            
            renderable.ModelMatrix = Transform.TransformMatrix *
                                      (Entity.Parent?.Transform.TransformMatrix ?? Matrix4x4.Identity);
            if (renderable.Material.Color.A < 1)
                Graphics.Renderer.DrawTranslucent(renderable);
            else
                Graphics.Renderer.DrawOpaque(renderable);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        
        for (int i = 0; i < _renderables.Length; i++)
            _renderables[i].Dispose();
    }
}