using System.Drawing;
using System.Reflection;
using Pie;

namespace Easel;

public struct GameSettings
{
    public Size Size;

    public string Title;

    public bool Resizable;

    public bool VSync;

    public GraphicsApi? Api;

    public GameSettings()
    {
        Size = new Size(1280, 720);

        string? name = Assembly.GetEntryAssembly()?.GetName().Name;
        Title = name == null ? "Easel Window" : name + " - Easel";
        Resizable = false;
        VSync = true;
        Api = null;
    }
}