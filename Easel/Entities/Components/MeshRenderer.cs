using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Easel.Primitives;
using Easel.Utilities;
using Pie;
using Pie.Utils;
using Silk.NET.Assimp;
using Material = Easel.Graphics.Material;
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

    private static Assimp _assimp;
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
    public unsafe MeshRenderer(string path, bool flipUvs = false)
    {
        // TODO is the incorrect loading of UVs for some models supposed to happen...??
        Logging.Log("Importing model with assimp...");
        if (_assimp == null)
            _assimp = Assimp.GetApi();
        Scene* scene = _assimp.ImportFile(path,
            (uint) PostProcessSteps.Triangulate | (uint) PostProcessSteps.FlipWindingOrder | (uint) PostProcessSteps.JoinIdenticalVertices | (uint) (flipUvs ? PostProcessSteps.FlipUVs : 0));
        if (scene == null || (scene->MFlags & Assimp.SceneFlagsIncomplete) != 0 || scene->MRootNode == null)
            Logging.Critical("Scene failed to import: " + _assimp.GetErrorStringS());

        _directory = Path.GetDirectoryName(path);

        _loadedTextures = new Dictionary<string, Texture2D>();
        
        List<Mesh> meshes = new List<Mesh>();
        ProcessNode(scene->MRootNode, scene, ref meshes);
        _meshes = meshes.ToArray();
    }

    private unsafe void ProcessNode(Node* node, Scene* scene, ref List<Mesh> meshes)
    {
        for (int i = 0; i < node->MNumMeshes; i++)
        {
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            meshes.Add(ProcessMesh(mesh, scene));
        }
        
        for (int i = 0; i < node->MNumChildren; i++)
            ProcessNode(node->MChildren[i], scene, ref meshes);
    }

    private unsafe Mesh ProcessMesh(Silk.NET.Assimp.Mesh* mesh, Scene* scene)
    {
        List<VertexPositionTextureNormal> vertices = new List<VertexPositionTextureNormal>();
        List<uint> indices = new List<uint>();
        List<Texture2D> textures = new List<Texture2D>();

        Console.WriteLine(mesh->MNumVertices);
        for (int i = 0; i < mesh->MNumVertices; i++)
            vertices.Add(new VertexPositionTextureNormal(mesh->MVertices[i], mesh->MTextureCoords[0] != null ? mesh->MTextureCoords[0][i].ToVector2() : Vector2.Zero, mesh->MNormals[i]));

        for (int i = 0; i < mesh->MNumFaces; i++)
        {
            Face face = mesh->MFaces[i];
            for (int f = 0; f < face.MNumIndices; f++)
                indices.Add(face.MIndices[f]);
        }

        Silk.NET.Assimp.Material* material = scene->MMaterials[mesh->MMaterialIndex];
        //textures.AddRange(LoadTextures(material, TextureType.Diffuse));
        //textures.AddRange(LoadTextures(material, TextureType.Specular));

        Texture2D[] diffuses = LoadTextures(material, TextureType.Diffuse);
        Texture2D[] speculars = LoadTextures(material, TextureType.Specular);
        Material mat = new Material(diffuses.Length > 0 ? diffuses[0] : Texture2D.Missing, speculars.Length > 0 ? speculars[0] : Texture2D.Void, Color.White, 32);
        return new Mesh(vertices.ToArray(), indices.ToArray(), mat);
    }

    private unsafe Texture2D[] LoadTextures(Silk.NET.Assimp.Material* material, TextureType type)
    {
        Texture2D[] textures = new Texture2D[_assimp.GetMaterialTextureCount(material, type)];
        for (int i = 0; i < textures.Length; i++)
        {
            AssimpString path;
            TextureMapping mapping;
            uint uvIndex = 0;
            float blend;
            TextureOp op;
            TextureMapMode mode;
            uint flags;
            _assimp.GetMaterialTexture(material, type, (uint) i, &path, &mapping, ref uvIndex, &blend, &op, &mode, &flags);
            string fullPath = Path.Combine(_directory, path.AsString);
            if (!_loadedTextures.TryGetValue(fullPath, out Texture2D texture))
            {
                texture = new Texture2D(fullPath);
                _loadedTextures.Add(fullPath, texture);
            }

            textures[i] = texture;
        }

        return textures;
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
                Matrix4x4.Identity, mesh.Material);
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
            Graphics.Renderer.DrawOpaque(renderable);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        
        for (int i = 0; i < _renderables.Length; i++)
            _renderables[i].Dispose();
    }
    
    public struct Mesh
    {
        public VertexPositionTextureNormal[] Vertices;
        public uint[] Indices;
        public Material Material;

        public Mesh(VertexPositionTextureNormal[] vertices, uint[] indices, Material material)
        {
            Vertices = vertices;
            Indices = indices;
            Material = material;
        }
    }
}