using System;
using System.Numerics;

namespace Isotope.Core.Math;

public static class Raycast
{
    public struct Ray
    {
        public Vector2 Origin;
        public Vector2 Direction;
    }

    public struct Segment
    {
        public Vector2 A;
        public Vector2 B;
    }

    public struct Intersection
    {
        public bool Hit;
        public Vector2 Point;
        public float Distance;
    }

    public static Intersection GetIntersection(Ray ray, Segment segment)
    {
        Vector2 r = ray.Direction;
        Vector2 s = segment.B - segment.A;

        float cross = r.X * s.Y - r.Y * s.X;

        if (Math.Abs(cross) < 1e-5) return new Intersection { Hit = false };

        Vector2 qp = segment.A - ray.Origin;
        float t = (qp.X * s.Y - qp.Y * s.X) / cross;
        float u = (qp.X * r.Y - qp.Y * r.X) / cross;

        if (t >= 0 && u >= 0 && u <= 1)
        {
            return new Intersection
            {
                Hit = true,
                Point = ray.Origin + r * t,
                Distance = t
            };
        }

        return new Intersection { Hit = false };
    }
}
