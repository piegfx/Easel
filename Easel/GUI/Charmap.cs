using System;
using System.Collections.Generic;
using Easel.Graphics;
using Easel.Math;
using Pie.Freetype;

namespace Easel.GUI;

public class Charmap : IDisposable
{
    public readonly Texture2D Texture;

    public readonly Dictionary<char, Character> Characters;

    public Charmap(Texture2D texture, Dictionary<char, Character> characters)
    {
        Texture = texture;
        Characters = characters;
    }

    public struct Character
    {
        public Rectangle Source;
        public Point Bearing;
        public int Advance;
    }

    public void Dispose()
    {
        Texture.Dispose();
    }
}