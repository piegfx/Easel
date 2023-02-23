using System;
using System.IO;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Primitives;
using Easel.Math;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestShadow : Scene
{
    private Model _model;
    
    protected override void Initialize()
    {
        base.Initialize();

        Camera.Main.ClearColor = Color.RebeccaPurple;
        //Bitmap bitmap = new Bitmap("/home/ollie/Pictures/ball.png");
        //Camera.Main.Skybox = new Skybox(bitmap, bitmap, bitmap, bitmap, bitmap, bitmap, SamplerState.PointClamp);
        Camera.Main.AddComponent(new NoClipCamera()
        {
            MoveSpeed = 5
        });

        GetEntity("Sun").GetComponent<DirectionalLight>().Direction = new Vector2<float>(0, 1);

        Texture2D texture2D = Content.Load<Texture2D>("Texture.png");
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
        }

        DDS dds = Content.Load<DDS>("DDS/24bitcolor-BC7");
        Texture2D ddsTexture = new Texture2D(dds.Bitmaps[0][0]);
        
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
}