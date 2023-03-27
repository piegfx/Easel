using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Primitives;
using Easel.Math;
using Easel.Scenes;
using Pie.Utils;
using Vector2 = System.Numerics.Vector2;

namespace Easel.Tests.TestScenes;

public class TestShadow : Scene
{
    private Model _model;
    
    protected override unsafe void Initialize()
    {
        base.Initialize();

        /*Scene* scene;
        ImpasseNative.iaLoadScene("/home/ollie/Downloads/Fox.gltf", &scene);

        List<ModelMesh> meshes = new List<ModelMesh>();
        Material material = new StandardMaterial(new Texture2D("/home/ollie/Downloads/Texture.png"));
        
        List<VertexPositionTextureNormalTangent> vptnts = new List<VertexPositionTextureNormalTangent>();

        for (int i = 0; i < (int) scene->NumMeshes; i++)
        {
            for (int j = 0; j < (int) scene->Meshes[i]->NumVertices; j++)
            {
                Mesh* mesh = scene->Meshes[i];
                vptnts.Add(new VertexPositionTextureNormalTangent(mesh->Vertices[j].Position, mesh->Vertices[j].TexCoord,
                    mesh->Vertices[j].Normal, mesh->Vertices[j].Tangent));
            }
            
            uint[] indices = new ReadOnlySpan<uint>(scene->Meshes[i]->Indices, (int) scene->Meshes[i]->NumIndices).ToArray();
            
            meshes.Add(new ModelMesh(new Graphics.Mesh[] { new Graphics.Mesh(vptnts.ToArray(), indices, material) }, Matrix4x4.Identity));
            vptnts.Clear();
        }

        _model = new Model(meshes.ToArray(), new[] { material });*/
        
        

        Camera.Main.ClearColor = Color.RebeccaPurple;
        //Bitmap bitmap = new Bitmap("/home/ollie/Pictures/ball.png");
        //Camera.Main.Skybox = new Skybox(bitmap, bitmap, bitmap, bitmap, bitmap, bitmap, SamplerState.PointClamp);
        Camera.Main.AddComponent(new NoClipCamera()
        {
            MoveSpeed = 5
        });

        GetEntity("Sun").GetComponent<DirectionalLight>().Direction = new Vector2T<float>(0, 1);

        /*Texture2D texture2D = Content.Load<Texture2D>("Texture.png");
        _model = Content.Load<Model>("Fox.gltf");

        for (int f = 0; f < 9; f++)
        {
            Entity entity = new Entity($"fox{f}", new Transform()
            {
                Position = new Vector3(f * 0.5f, 0, 0),
                Scale = new Vector3(1 / 100f)
            });
            Material material;
            if (f == 0)
                material = new StandardMaterial(texture2D);
            else
                material = new TranslucentStandardMaterial(texture2D)
                {
                    AlbedoColor = new Color(1.0f, 1.0f, 1.0f, 1 - f / 10f)
                };
            
            for (int i = 0; i < _model.Meshes.Length; i++)
            {
                ref ModelMesh mMesh = ref _model.Meshes[i];
                for (int j = 0; j < mMesh.Meshes.Length; j++)
                {
                    mMesh.Meshes[j].Material = material;
                }
            }
            entity.AddComponent(new ModelRenderer(_model));
            AddEntity(entity);
        }*/

        _model = Content.Load<Model>("Fox");
        Entity test = new Entity("test", new Transform()
        {
            Scale = new Vector3(0.01f)
        });
        test.AddComponent(new ModelRenderer(_model));
        AddEntity(test);

        //DDS dds = Content.Load<DDS>("DDS/24bitcolor-BC7");
        //Texture2D ddsTexture = new Texture2D(dds.Bitmaps[0][0]);
        Texture2D ddsTexture = Content.Load<Texture2D>("DDS/Compressed/24bitcolor-BC7");
        
        Entity cube = new Entity("cube", new Transform()
        {
            Position = new Vector3(2f, -0.05f, 0),
            Scale = new Vector3(10, 0.1f, 10)
        });
        cube.AddComponent(new ModelRenderer(new Cube(),
            new StandardMaterial(ddsTexture)
                { RasterizerState = RasterizerState.CullClockwise }));
        AddEntity(cube);

        Material material0 = new StandardMaterial(Texture2D.Black);
        Material material1 = new StandardMaterial(Texture2D.Black);
        Material material2 = new StandardMaterial(Texture2D.Black);
        Material material3 = new StandardMaterial(Texture2D.Black);
        Material material4 = new StandardMaterial(Texture2D.Black);
        Material material5 = new StandardMaterial(Texture2D.Black);
        Material material6 = new StandardMaterial(Texture2D.Black);
        Material material7 = new StandardMaterial(Texture2D.Black);
        
        material0.Dispose();
        material1.Dispose();
        material2.Dispose();
        material3.Dispose();
        material4.Dispose();
        material5.Dispose();
        material6.Dispose();
        material7.Dispose();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _model.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VertexPositionColorTextureNormalTangentBitangent
    {
        public Vector3 Position;
        public Vector4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Bitangent;
    }

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct Mesh
    {
        public VertexPositionColorTextureNormalTangentBitangent* Vertices;
        public nuint NumVertices;
        public uint* Indices;
        public nuint NumIndices;
    }

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct Scene
    {
        public Mesh** Meshes;
        public nuint NumMeshes;
    }

    private static class ImpasseNative
    {
        public const string ImpasseName = "libimpasse";
        
        [DllImport(ImpasseName)]
        public static extern unsafe void iaLoadScene(string path, Scene** scene);
    }
}