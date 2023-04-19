using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Easel.Core;
using Easel.Graphics.Materials;
using Easel.Math;
using Silk.NET.Assimp;
using Material = Easel.Graphics.Materials.Material;

namespace Easel.Graphics;

/// <summary>
/// Represents a model and scene graph.
/// </summary>
public unsafe class Model : IDisposable
{
    private static Assimp _assimp;

    public ModelMesh[] Meshes;

    public Material[] Materials;

    public Model(ModelMesh[] meshes, Material[] materials)
    {
        Meshes = meshes;
        Materials = materials;
    }

    public Model(string path, bool flipUvs = true)
    {
        // TODO: IMPORTANT! Transparent material support in model, loading embedded textures
        
        _assimp ??= Assimp.GetApi();

        Scene* scene = _assimp.ImportFile(path,
            (uint) PostProcessSteps.Triangulate |
            (uint) PostProcessSteps.GenerateUVCoords | (uint) PostProcessSteps.JoinIdenticalVertices |
            (uint) PostProcessSteps.CalculateTangentSpace | (uint) PostProcessSteps.PreTransformVertices |
            (uint) PostProcessSteps.GenerateSmoothNormals | (flipUvs ? (uint) PostProcessSteps.FlipUVs : 0));

        if (scene == null || (scene->MFlags & Assimp.SceneFlagsIncomplete) != 0 ||
            scene->MRootNode == null)
        {
            throw new EaselException("Failed to load assimp: " + _assimp.GetErrorStringS());
        }

        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        Texture2D[] LoadTexturesForMatType(Silk.NET.Assimp.Material* material, TextureType type)
        {
            Texture2D[] texts = new Texture2D[_assimp.GetMaterialTextureCount(material, type)];
            
            for (int i = 0; i < texts.Length; i++)
            {
                if (i > 0)
                    throw new NotImplementedException("Currently materials can only support one texture per type.");
                
                AssimpString aPath;
                TextureMapping mapping;
                uint uvIndex;
                float blend;
                TextureOp op;
                TextureMapMode mode;
                uint flags;

                _assimp.GetMaterialTexture(material, type, (uint) i, &aPath, &mapping, &uvIndex, &blend, &op, &mode,
                    &flags);

                string assimpPath = aPath.AsString;
                if (!textures.TryGetValue(assimpPath, out Texture2D texture))
                {
                    if (assimpPath[0] == '*')
                    {
                        Silk.NET.Assimp.Texture* aTexture = scene->MTextures[int.Parse(assimpPath[1..])];
                        // Texture is in a png/jpeg/whatever format, instead of raw data.
                        if (aTexture->MHeight == 0)
                        {
                            // The width gives the size of the file in bytes.
                            ReadOnlySpan<byte> tData = new ReadOnlySpan<byte>((void*) aTexture->PcData, (int) aTexture->MWidth); 
                            byte[] texData = tData.ToArray();
                            Bitmap bitmap = new Bitmap(texData);
                            texture = new Texture2D(bitmap);
                        }
                    }
                    else
                    {
                        string fullPath = Path.Combine(Path.GetDirectoryName(path), assimpPath);
                        texture = new Texture2D(fullPath);
                    }

                    textures.Add(assimpPath, texture);
                }

                texts[i] = texture;
            }

            return texts;
        }

        Materials = new Material[scene->MNumMaterials];
        for (int i = 0; i < scene->MNumMaterials; i++)
        {
            Silk.NET.Assimp.Material* material = scene->MMaterials[i];
            
            Texture2D[] albedo = LoadTexturesForMatType(material, TextureType.Diffuse);
            Texture2D[] normal = LoadTexturesForMatType(material, TextureType.Normals);
            Texture2D[] metallic = LoadTexturesForMatType(material, TextureType.Metalness);
            Texture2D[] roughness = LoadTexturesForMatType(material, TextureType.DiffuseRoughness);
            Texture2D[] ao = LoadTexturesForMatType(material, TextureType.AmbientOcclusion);

            MaterialProperty* property;
            _assimp.GetMaterialProperty(material, Assimp.MatkeyTexblendBase, 0, 0, &property);
            
            if (property != null)
                Console.WriteLine((BlendMode) BitConverter.ToInt32(new ReadOnlySpan<byte>(property->MData, (int) property->MDataLength)));
            
            Materials[i] = new StandardMaterial(albedo.Length > 0 ? albedo[0] : Texture2D.White,
                normal.Length > 0 ? normal[0] : Texture2D.EmptyNormal,
                metallic.Length > 0 ? metallic[0] : Texture2D.Black,
                roughness.Length > 0 ? roughness[0] : Texture2D.Black, ao.Length > 0 ? ao[0] : Texture2D.White);
        }

        List<VertexPositionTextureNormalTangent> vptnts = new List<VertexPositionTextureNormalTangent>();
        List<uint> indices = new List<uint>();

        Mesh[] meshes = new Mesh[scene->MNumMeshes];
        for (int i = 0; i < scene->MNumMeshes; i++)
        {
            vptnts.Clear();
            indices.Clear();
            
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[i];

            for (int v = 0; v < mesh->MNumVertices; v++)
                vptnts.Add(new VertexPositionTextureNormalTangent((Vector3T<float>) mesh->MVertices[v], (Vector2T<float>) mesh->MTextureCoords[0][v].ToVector2(), mesh->MNormals == null ? Vector3T<float>.Zero : (Vector3T<float>) mesh->MNormals[v], mesh->MTangents == null ? Vector3T<float>.Zero : (Vector3T<float>) mesh->MTangents[0]));

            for (int f = 0; f < mesh->MNumFaces; f++)
            {
                Face face = mesh->MFaces[f];
                for (int t = 0; t < face.MNumIndices; t++)
                    indices.Add(face.MIndices[t]);
            }

            meshes[i] = new Mesh(vptnts.ToArray(), indices.ToArray(), Materials[mesh->MMaterialIndex]);
        }

        List<ModelMesh> mmeshes = new List<ModelMesh>();
        ProcessNode(scene->MRootNode, mmeshes, meshes, Matrix4x4.Identity);
        Meshes = mmeshes.ToArray();
    }

    private void ProcessNode(Node* node, List<ModelMesh> mmeshes, Mesh[] allMeshes, Matrix4x4 transform)
    {
        Mesh[] meshes = new Mesh[node->MNumMeshes];
        for (int i = 0; i < node->MNumMeshes; i++)
            meshes[i] = allMeshes[node->MMeshes[i]];
        transform = node->MTransformation * transform;
        mmeshes.Add(new ModelMesh(meshes, Matrix4x4.Identity));
        
        for (int i = 0; i < node->MNumChildren; i++)
            ProcessNode(node->MChildren[i], mmeshes, allMeshes, transform);
    }

    public void Dispose()
    {
        foreach (Material material in Materials)
            material.Dispose();
    }
}