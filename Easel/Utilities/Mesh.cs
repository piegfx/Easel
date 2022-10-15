using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Easel.Graphics;
using Easel.Math;
using Pie.Utils;
using Silk.NET.Assimp;
using Material = Easel.Graphics.Material;

namespace Easel.Utilities;

public struct Mesh
{
    private static Assimp _assimp;
    
    public VertexPositionTextureNormalTangent[] Vertices;
    public uint[] Indices;
    public Material Material;
    
    public Mesh(VertexPositionTextureNormalTangent[] vertices, uint[] indices, Material material)
    {
        Vertices = vertices;
        Indices = indices;
        Material = material;
    }
    
    public static unsafe Mesh[] LoadFromFile(string path, bool flipUvs = false)
    {
        // TODO is the incorrect loading of UVs for some models supposed to happen...??
        Logging.Log("Importing model with assimp...");
        if (_assimp == null)
            _assimp = Assimp.GetApi();
        Scene* scene = _assimp.ImportFile(path,
            (uint) PostProcessSteps.Triangulate | (uint) PostProcessSteps.FlipWindingOrder | (uint) PostProcessSteps.CalculateTangentSpace | (uint) PostProcessSteps.JoinIdenticalVertices | (uint) (flipUvs ? PostProcessSteps.FlipUVs : 0) | (uint) PostProcessSteps.GenerateNormals);
        if (scene == null || (scene->MFlags & Assimp.SceneFlagsIncomplete) != 0 || scene->MRootNode == null)
            Logging.Fatal("Scene failed to import: " + _assimp.GetErrorStringS());
        
        string directory = Path.GetDirectoryName(path);

        Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
        
        List<Mesh> meshes = new List<Mesh>();
        ProcessNode(scene->MRootNode, scene, ref meshes, directory, ref loadedTextures);
        return meshes.ToArray();
    }
    
    private static unsafe void ProcessNode(Node* node, Scene* scene, ref List<Mesh> meshes, string directory, ref Dictionary<string, Texture2D> loadedTextures)
    {
        for (int i = 0; i < node->MNumMeshes; i++)
        {
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[node->MMeshes[i]];
            meshes.Add(ProcessMesh(mesh, scene, directory, ref loadedTextures));
        }
        
        for (int i = 0; i < node->MNumChildren; i++)
            ProcessNode(node->MChildren[i], scene, ref meshes, directory, ref loadedTextures);
    }
    
    private static unsafe Mesh ProcessMesh(Silk.NET.Assimp.Mesh* mesh, Scene* scene, string directory, ref Dictionary<string, Texture2D> loadedTextures)
    {
        List<VertexPositionTextureNormalTangent> vertices = new List<VertexPositionTextureNormalTangent>();
        List<uint> indices = new List<uint>();
        List<Texture2D> textures = new List<Texture2D>();
        
        for (int i = 0; i < mesh->MNumVertices; i++)
            vertices.Add(new VertexPositionTextureNormalTangent(mesh->MVertices[i], mesh->MTextureCoords[0] != null ? mesh->MTextureCoords[0][i].ToVector2() : Vector2.Zero, mesh->MNormals[i], mesh->MTangents[i]));

        for (int i = 0; i < mesh->MNumFaces; i++)
        {
            Face face = mesh->MFaces[i];
            for (int f = 0; f < face.MNumIndices; f++)
                indices.Add(face.MIndices[f]);
        }

        Silk.NET.Assimp.Material* material = scene->MMaterials[mesh->MMaterialIndex];
        //textures.AddRange(LoadTextures(material, TextureType.Diffuse));
        //textures.AddRange(LoadTextures(material, TextureType.Specular));

        MaterialProperty* shininessProp;
        _assimp.GetMaterialProperty(material, Assimp.MatkeyShininess, 0, 0, &shininessProp);

        float shininess =
            BitConverter.ToSingle(new ReadOnlySpan<byte>(shininessProp->MData, (int) shininessProp->MDataLength));

        Texture2D[] diffuses = LoadTextures(material, TextureType.Diffuse, directory, ref loadedTextures);
        Texture2D[] speculars = LoadTextures(material, TextureType.Specular, directory, ref loadedTextures);
        Texture2D[] normals = LoadTextures(material, TextureType.Height, directory, ref loadedTextures);
        Material mat = new Material(diffuses.Length > 0 ? diffuses[0] : Texture2D.Blank,
            speculars.Length > 0 ? speculars[0] : diffuses.Length > 0 ? diffuses[0] : null,
            normals.Length > 0 ? normals[0] : null, Color.White, shininess);
        return new Mesh(vertices.ToArray(), indices.ToArray(), mat);
    }
    
    private static unsafe Texture2D[] LoadTextures(Silk.NET.Assimp.Material* material, TextureType type, string directory, ref Dictionary<string, Texture2D> loadedTextures)
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
            string fullPath = Path.Combine(directory, path.AsString);
            if (!loadedTextures.TryGetValue(fullPath, out Texture2D texture))
            {
                texture = new Texture2D(fullPath);
                loadedTextures.Add(fullPath, texture);
            }

            textures[i] = texture;
        }

        return textures;
    }
}