using System;
using System.Numerics;

namespace Isotope.Core.Math;

/// <summary>
/// Provides functionality for raycasting.
/// </summary>
public static class Raycast
{
    /// <summary>
    /// Represents a ray with an origin and a direction.
    /// </summary>
    public struct Ray
    {
        /// <summary>
        /// The origin of the ray.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public Vector2 Direction;
    }

    /// <summary>
    /// Represents a line segment with two endpoints.
    /// </summary>
    public struct Segment
    {
        /// <summary>
        /// The first endpoint of the segment.
        /// </summary>
        public Vector2 A;

        /// <summary>
        /// The second endpoint of the segment.
        /// </summary>
        public Vector2 B;
    }

    /// <summary>
    /// Represents the result of an intersection between a ray and a segment.
    /// </summary>
    public struct Intersection
    {
        /// <summary>
        /// A value indicating whether the intersection occurred.
        /// </summary>
        public bool Hit;

        /// <summary>
        /// The point of intersection.
        /// </summary>
        public Vector2 Point;

        /// <summary>
        /// The distance from the ray's origin to the intersection point.
        /// </summary>
        public float Distance;
    }

    /// <summary>
    /// Gets the intersection between a ray and a segment.
    /// </summary>
    /// <param name="ray">The ray to test for intersection.</param>
    /// <param name="segment">The segment to test for intersection.</param>
    /// <returns>An <see cref="Intersection"/> object representing the result of the intersection test.</returns>
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
