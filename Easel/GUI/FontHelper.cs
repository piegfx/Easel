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
        int posX = 0, posY = 0;
        for (char c = rangeMin; c < rangeMax; c++)
        {
            Character chr = face.Characters[c];
            if (posX + chr.Width >= texture.Size.Width)
            {
                posX = 0;
                posY += face.Size;
                if (posY + face.Size >= texture.Size.Height)
                    throw new EaselException("Font texture is too small to fit all characters in range.");
            }
            texture.SetData(posX, posY, chr.Width, chr.Height, chr.Bitmap);
            characters.Add(c, new Charmap.Character()
            {
                Source = new Rectangle(posX, posY, chr.Width, chr.Height),
                Bearing = new Point(chr.BitmapLeft, chr.BitmapTop),
                Advance = chr.Advance
            });
            posX += chr.Advance + 2;
        }

        return new Charmap(texture, characters);
    }

    static FontHelper()
    {
        FreeType = new FreeType();
    }
}