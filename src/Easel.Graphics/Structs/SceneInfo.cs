using System.Runtime.InteropServices;

namespace Easel.Graphics.Structs;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SceneInfo
{
    public float AmbientMultiplier;

    private fixed float _padding[3];

    public SceneInfo(float ambientMultiplier)
    {
        AmbientMultiplier = ambientMultiplier;
    }
}