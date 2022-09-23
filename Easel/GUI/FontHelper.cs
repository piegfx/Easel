using System;
using System.Collections.Generic;
using Easel.Graphics;
using Easel.Math;
using Pie.Freetype;

namespace Easel.GUI;

public static class FontHelper
{
    public static readonly FreeType FreeType;

    public static Charmap GenerateCharmapInRange(Face face, char rangeMin, char rangeMax)
    {
        // Padding reduces the risk of "bleed" by adding a gap between characters;
        const int padding = 2;
        
        int size = 0;
        int totalSize = (rangeMax - rangeMin) * face.Size * face.Size;
        size = (int) MathF.Sqrt(totalSize);
        size--;
        size |= size >> 1;
        size |= size >> 2;
        size |= size >> 4;
        size |= size >> 8;
        size |= size >> 16;
        size++;
        
        Logging.Log($"Font texture size calculated to be {size}x{size} pixels.");
        int width = size;
        int height = size;
        byte[] initialData = new byte[width * height * 4];
        Texture2D texture = new Texture2D(width, height, initialData, autoDispose: false);
        Dictionary<char, Charmap.Character> characters = new Dictionary<char, Charmap.Character>();
        Point pos = Point.Zero;
        for (char c = rangeMin; c < rangeMax; c++)
        {
            AddCharacter(face, c, ref pos, ref texture, ref characters, padding);
        }

        if (!characters.ContainsKey('?'))
        {
            AddCharacter(face, '?', ref pos, ref texture, ref characters, padding);
        }

        return new Charmap(texture, characters);
    }

    private static void AddCharacter(Face face, char c, ref Point pos, ref Texture2D texture, ref Dictionary<char, Charmap.Character> characters, int padding)
    {
        Character chr = face.Characters[c];
        if (pos.X + chr.Width >= texture.Size.Width)
        {
            pos.X = 0;
            pos.Y += face.Size + padding;
            if (pos.Y + face.Size >= texture.Size.Height)
                throw new EaselException("Font texture is too small to fit all characters in range.");
        }
        texture.SetData(pos.X, pos.Y, chr.Width, chr.Height, chr.Bitmap);
        characters.Add(c, new Charmap.Character()
        {
            Source = new Rectangle(pos.X, pos.Y, chr.Width, chr.Height),
            Bearing = new Point(chr.BitmapLeft, chr.BitmapTop),
            Advance = chr.Advance
        });
        pos.X += chr.Advance + padding;
    }

    static FontHelper()
    {
        FreeType = new FreeType();
    }
}