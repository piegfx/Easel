using System;
using System.IO;
using System.Numerics;
using Easel.Audio;
using Easel.Content;
using Easel.Content.Builder;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Imgui;
using Easel.Math;
using Easel.Scenes;
using Newtonsoft.Json;
using Sprite = Easel.Entities.Components.Sprite;

namespace Easel.Tests.TestScenes;

public class Test2D : Scene
{
    private SpriteRenderer.SpriteVertex[] _vertices;
    private uint[] _indices;
    private Texture2D _texture;

    private FilePicker _filePicker;

    protected override void Initialize()
    {
        base.Initialize();

        /*// Create a new content definition, using the content directory named "Content".
        // This creates it from a builder, so we can add content as we please.
        // When providing a path to the content, it only needs to be relative.
        // No need to include the "Content/" or anything like that. It will work that out automatically.
        ContentDefinition definition = new ContentBuilder("Content")
            // Load an image as is. Its name will be "front"
            .Add(new ImageContent("front.jpg"))
            // Directories are supported too. This one's name will be "DDS/24bitcolor-BC7"
            .Add(new ImageContent("DDS/24bitcolor-BC7.dds"))
            // Duplicates are not allowed (aka they have the same name, but different file extension).
            // To get around this, assign them a friendly name.
            .Add(new SoundContent("Audio/help.ogg") { FriendlyName = "Audio/helpogg" } )
            .Add(new SoundContent("Audio/help.wav") { FriendlyName = "Audio/helpwav" })
            // Some content types, such as model, allow for extra parameters.
            .Add(new ModelContent("Fox.gltf", true))
            .Build();*/

        //ContentDefinition definition = ContentBuilder.FromDirectory("Content").Build(DuplicateHandling.Overwrite);

        // Before we can use the content, we must add the definition.
        //Content.AddContent(definition);

        // Load as usual!
        _texture = Content.Load<Texture2D>("DDS/Compressed/24bitcolor-BC7");

        Sound sound = Content.Load<Sound>("Audio/help");
        Console.WriteLine(sound.SoundType);
        sound.Play();

        //File.WriteAllBytes("/home/ollie/Pictures/ETF/test.etf", ETF.CreateEtf(new Bitmap("/home/ollie/Pictures/24bitcolor.png"), customData: "(C) SPACEBOX 2023"));

        //ETF tex = new ETF(File.ReadAllBytes("/home/ollie/Pictures/ETF/test.etf"));
        
        //Console.WriteLine("loading dds");
        //DDS tex = new DDS(File.ReadAllBytes("/home/ollie/Downloads/RubberCuboidFloor/RubberCuboidFloor_4K_BaseColor.dds"));
        
        Camera.Main.UseOrtho2D();
        Camera.Main.ClearColor = Color.CornflowerBlue;
        
        _vertices = new SpriteRenderer.SpriteVertex[]
        {
            new(new Vector2(0, 0), new Vector2(0, 0), Color.White),
            new(new Vector2(1024, 0), new Vector2(1, 0), Color.White),
            new(new Vector2(1024, 1024), new Vector2(1, 1), Color.White),
            new(new Vector2(0, 1024), new Vector2(0, 1), Color.White),
        };
        
        _indices = new uint[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        /*_font = Content.Load<Font>("Abel-Regular");

        const uint size = 128;
        const string text = "docs.piegfx.com";
        
        _rt = new RenderTarget(_font.MeasureString(size, text) + new Size<int>(0, 10));
        
        Graphics.SetRenderTarget(_rt);
        Graphics.Clear(Color.Transparent);
        Graphics.SpriteRenderer.Begin();
        _font.Draw(Graphics.SpriteRenderer, size, text, Vector2T<int>.Zero, Color.White);
        Graphics.SpriteRenderer.End();
        Graphics.SetRenderTarget(null);

        */

        //_texture = new Texture2D(tex.Bitmaps[0][0], SamplerState.LinearClamp);
        
        /*Entity sprite = new Entity();
        sprite.AddComponent(new Sprite(texture));
        AddEntity(sprite);*/

        //UI.Theme.Font = new Font("/home/ollie/Documents/Abel-Regular.ttf", new FontOptions() { IsAntialiased = true });
        //UI.Add("test", new Label(new Position(Anchor.CenterCenter), "Stuff", 200, Color.Red));

        _filePicker = new FilePicker(FilePickerType.Open);
    }

    private float _f;

    protected override void Update()
    {
        base.Update();
        
        _filePicker.Update();
    }

    protected override void Draw()
    {
        base.Draw();

        //_f += Time.DeltaTime;

        Graphics.SpriteRenderer.Begin(transform: Matrix4x4.CreateTranslation(-_f, 0, 0));
        Graphics.SpriteRenderer.DrawVertices(_texture, _vertices, _indices);
        Graphics.SpriteRenderer.End();

        string text = "What??";
        uint size = 100;

        Graphics.SpriteRenderer.Begin();
        Position position = new Position(Anchor.TopLeft);
        Vector2T<int> realPos = position.CalculatePosition(Graphics.Viewport, UI.DefaultStyle.Font.MeasureString(size, text));
        UI.DefaultStyle.Font.Draw(Graphics.SpriteRenderer, size, text, realPos, Color.White, EaselMath.ToRadians(20), Vector2.Zero, new Vector2(5, 1));
        
        Graphics.SpriteRenderer.DrawRectangle(new Vector2(100), new Size<float>(100), 2, 20, Color.White,
            Color.Black, 0, Vector2.One);
        
        Graphics.SpriteRenderer.End();

        //Camera.Main.ClearColor = Color.FromHsv(200, 0.5f, 0.75f);
        //Camera.Main.Transform.Position.X += 1;
    }
}