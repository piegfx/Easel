using Easel.Graphics.Materials;

namespace Easel.Utilities;

public struct MaterialMesh
{
    public Mesh Mesh;
    public Material Material;

    public MaterialMesh(Mesh mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }
}