using System;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
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

        Camera.Main.ClearColor = Color.CornflowerBlue;
        Bitmap bitmap = new Bitmap("/home/ollie/Pictures/ball.png");
        Camera.Main.Skybox = new Skybox(bitmap, bitmap, bitmap, bitmap, bitmap, bitmap, SamplerState.PointClamp);
        Camera.Main.AddComponent(new NoClipCamera()
        {
            MoveSpeed = 5
        });

        GetEntity("Sun").GetComponent<DirectionalLight>().Direction = new Vector2<float>(0, 1);

        Entity entity = new Entity(new Transform()
        {
            Position = new Vector3(0, 0, -3),
        });
        //entity.AddComponent(new ModelRenderer(new Cube(), new StandardMaterial()));
        _model = new Model("/home/ollie/Downloads/Fox.gltf");
        Material material = new TranslucentStandardMaterial(new Texture2D("/home/ollie/Downloads/Texture.png"))
        {
            AlbedoColor = new Color(1.0f, 1.0f, 1.0f, 0.5f)
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

    public override void Dispose()
    {
        base.Dispose();
        
        _model.Dispose();
    }
}