using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.GUI.BBCode;
using Easel.Math;
using Pie.Freetype;

namespace Easel.GUI;

public class Font : IDisposable
{
    public readonly Face Face;

    private Dictionary<uint, Charmap> _charmaps;

    public Font(string path)
    {
        Face = FontHelper.FreeType.CreateFace(path, 0);

        _charmaps = new Dictionary<uint, Charmap>();
    }

    public void Draw(uint size, string text, Vector2 position)
    {
        if (!_charmaps.TryGetValue(size, out Charmap charmap))
        {
            Face.Size = (int) size;
            charmap = FontHelper.GenerateCharmapInRange(Face, (char) 0, (char) 127);
            _charmaps.Add(size, charmap);
        }

        Vector2 pos = position;
        int largestChar = 0;
        foreach (char c in text)
        {
            Charmap.Character chr = charmap.Characters[c];
            if (chr.Source.Height > largestChar)
                largestChar = chr.Bearing.Y;
        }
        pos.Y += largestChar;

        foreach (char c in text)
        {
            switch (c)
            {
                case '\n':
                    pos.Y += size;
                    pos.X = position.X;
                    continue;
            }
            
            Charmap.Character chr = charmap.Characters[c];
            Vector2 charPos = new Vector2(pos.X + chr.Bearing.X,
                pos.Y - chr.Source.Height + (chr.Source.Height - chr.Bearing.Y));
            SpriteRenderer.Draw(charmap.Texture, charPos, chr.Source, Color.White);
            pos.X += chr.Advance;
        }
    }

    public void DrawBBCode(uint size, string text, Vector2 position)
    {
        if (!_charmaps.TryGetValue(size, out Charmap charmap))
        {
            Face.Size = (int) size;
            charmap = FontHelper.GenerateCharmapInRange(Face, (char) 0, (char) 127);
            _charmaps.Add(size, charmap);
        }

        BBCodeInstruction[] bbCode = BBCodeParser.Parse(text);

        Vector2 pos = position;
        int largestChar = 0;
        foreach (char c in text)
        {
            Charmap.Character chr = charmap.Characters[c];
            if (chr.Source.Height > largestChar)
                largestChar = chr.Bearing.Y;
        }
        pos.Y += largestChar;
        
        Color initialColor = Color.White;
        ref Color currentColor = ref initialColor;

        foreach (BBCodeInstruction instruction in bbCode)
        {
            if (instruction == null)
                continue;
            
            switch (instruction.InstructionType)
            {
                case InstructionType.Text:
                    TextInstruction tInst = (TextInstruction) instruction;
                    foreach (char c in tInst.Text)
                    {
                        switch (c)
                        {
                            case '\n':
                                pos.Y += size;
                                pos.X = position.X;
                                continue;
                        }

                        Charmap.Character chr = charmap.Characters[c];
                        Vector2 charPos = new Vector2(pos.X + chr.Bearing.X,
                            pos.Y - chr.Source.Height + (chr.Source.Height - chr.Bearing.Y));
                        SpriteRenderer.Draw(charmap.Texture, charPos, chr.Source, currentColor);
                        pos.X += chr.Advance;
                    }

                    break;
                case InstructionType.Color:
                    ColorInstruction cInst = (ColorInstruction) instruction;
                    if (cInst.IsExiting)
                        currentColor = ref initialColor;
                    else
                        currentColor = ref cInst.Color;
                    break;
            }
        }
    }

    public void Dispose()
    {
        foreach ((_, Charmap charmap) in _charmaps)
            charmap.Dispose();
    }
}