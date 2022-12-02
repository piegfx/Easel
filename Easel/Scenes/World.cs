using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Lighting;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.Scenes;

/// <summary>
/// Represents various scene-wide settings, such as clear color, and skybox.
/// </summary>
public class World
{
    public Color ClearColor;

    public DirectionalLight Sun;

    public Skybox Skybox;

    // TODO: Probably implement sampler states per texture, which utilizes a global sampler state system to avoid creating
    // more sampler states than is necessary.
    public SpriteRenderMode SpriteRenderMode;

    public World()
    {
        ClearColor = Color.Black;
        Sun = new DirectionalLight(new Vector2(MathF.PI / 4, MathF.PI / 4), new Color(20, 20, 20, 255), new Color(180, 180, 180, 255),
            new Color(255, 255, 255, 255));
        Skybox = null;
        SpriteRenderMode = SpriteRenderMode.Linear;
    }
}