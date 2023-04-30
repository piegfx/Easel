using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Primitives;
using Easel.Graphics.Renderers;
using Easel.Headless.Entities.Components;
using Material = Easel.Graphics.Materials.Material;
using Mesh = Easel.Graphics.Mesh;

namespace Easel.Entities.Components;

/// <summary>
/// The bog-standard 3D model renderer for an entity.
/// </summary>
public class ModelRenderer : Component
{
    private Renderable[] _renderables;
    private Matrix4x4[] _transforms;

    public ModelRenderer(Model model)
    {
        List<Renderable> renderables = new List<Renderable>();
        List<Matrix4x4> transforms = new List<Matrix4x4>();

        foreach (ModelMesh mmesh in model.Meshes)
        {
            foreach (Mesh mesh in mmesh.Meshes)
            {
                renderables.Add(Renderable.CreateFromMesh(mesh));
                transforms.Add(mmesh.Transform);
            }
        }

        _renderables = renderables.ToArray();
        _transforms = transforms.ToArray();
    }

    public ModelRenderer(IPrimitive primitive, Material material)
    {
        _renderables = new[] { Renderable.CreateFromMesh(new Mesh(primitive.Vertices, primitive.Indices, material)) };
        _transforms = new[] { Matrix4x4.Identity };
    }

    protected override void Draw()
    {
        base.Draw();
        Matrix4x4 world = Transform.TransformMatrix *
                          (Entity.Parent?.Transform.TransformMatrix ?? Matrix4x4.Identity);
        for (int i = 0; i < _renderables.Length; i++)
            EaselGame.Instance.Graphics.Renderer.Draw(_renderables[i], Matrix4x4.Transpose(_transforms[i]) * world);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        for (int i = 0; i < _renderables.Length; i++)
            _renderables[i].Dispose();
    }
}