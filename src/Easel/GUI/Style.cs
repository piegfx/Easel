using Easel.Graphics;
using Easel.Math;

namespace Easel.GUI;

public struct Style
{
    public Color TextColor;

    public PanelStyle Panel;

    public Texture BackgroundTexture;

    public Font Font;

    public Style()
    {
        TextColor = Color.Black;
        Panel = new PanelStyle();
        
        Font = null;
    }

    public struct PanelStyle
    {
        public Color BackgroundColor;

        public Color BorderColor;

        public int BorderWidth;

        public float BorderRadius;

        public PanelStyle()
        {
            BackgroundColor = Color.White;
            BorderColor = Color.Black;
            BorderWidth = 2;
            BorderRadius = 20f;
        }
    }
}