namespace Easel.Imgui;

public struct FontInfo
{
    public string Path;
    public float Size;

    public FontInfo(string path, float size)
    {
        Path = path;
        Size = size;
    }
}