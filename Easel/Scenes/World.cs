using System.Numerics;
using Easel.Graphics.Lighting;
using Easel.Math;

namespace Easel.Scenes;

/// <summary>
/// Represents various scene-wide settings, such as clear color, and skybox.
/// </summary>
public class World
{
    public Color ClearColor;

    public DirectionalLight Sun;

    public World()
    {
        ClearColor = Color.Black;
        Sun = new DirectionalLight(new Vector3(0.4f, -0.25f, 1), new Color(20, 20, 20, 255), new Color(180, 180, 180, 255),
            new Color(255, 255, 255, 255));
    }
}