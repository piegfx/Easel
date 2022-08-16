using System;
using Easel.Utilities;
using Pie;

namespace Easel.Graphics;

public class Texture2D : Texture
{
    public Texture2D(string path, bool autoDispose = true) : base(autoDispose)
    {
        Bitmap bitmap = new Bitmap(path);
        GraphicsDevice device = EaselGame.Device;
        PieTexture = device.CreateTexture(bitmap.Size.Width, bitmap.Size.Height, bitmap.Format, bitmap.Data);
    }
}