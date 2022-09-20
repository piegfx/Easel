using Pie.Freetype;

namespace Easel.GUI;

public static class FontHelper
{
    public static readonly FreeType FreeType;

    static FontHelper()
    {
        FreeType = new FreeType();
    }
}