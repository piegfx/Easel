using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics.Renderers;

namespace Easel.Utilities;

public struct Triangulation
{
    public SpriteRenderer.SpriteVertex[] Vertices;
    public uint[] Indices;

    public Triangulation(params Vector2[] points)
    {
        List<Triangle> triangles = new List<Triangle>();
        List<Triangle> badTriangles = new List<Triangle>();
        List<Triangle> polygon = new List<Triangle>();

        foreach (Vector2 point in points)
        {
            badTriangles.Clear();
            
            
        }

        Vertices = null;
        Indices = null;
    }

    private struct Triangle
    {
        public Vector2 Point1;
        public Vector2 Point2;
        public Vector2 Point3;

        public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
        }
    }
}