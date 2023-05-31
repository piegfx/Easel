using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct Material
{
    public Color AlbedoMultiplier;

    public Material(Color albedoMultiplier)
    {
        AlbedoMultiplier = albedoMultiplier;
    }
}