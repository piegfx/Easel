using System.Numerics;

namespace Easel.Math;

public struct Frustum
{
    public Plane TopFace;
    public Plane BottomFace;

    public Plane RightFace;
    public Plane LeftFace;

    public Plane FarFace;
    public Plane NearFace;

    public Frustum(Plane topFace, Plane bottomFace, Plane rightFace, Plane leftFace, Plane farFace, Plane nearFace)
    {
        TopFace = topFace;
        BottomFace = bottomFace;
        RightFace = rightFace;
        LeftFace = leftFace;
        FarFace = farFace;
        NearFace = nearFace;
    }

    public struct Plane
    {
        public Vector3 Normal;

        public float Distance;

        public Plane(Vector3 normal, float distance)
        {
            Normal = normal;
            Distance = distance;
        }
    }
}