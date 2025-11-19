using System;
using System.Collections.Generic;
using System.Numerics;
using Isotope.Core.Map;
using Isotope.Core.Math;
using Isotope.Core.Tiles;

namespace Isotope.Client.Rendering;

/// <summary>
/// A class that calculates a light polygon for a given light source, taking into account shadows from walls.
/// </summary>
public class ShadowCaster
{
    private List<Vector2> _pointsOfInterest = new List<Vector2>(512);
    private List<Raycast.Segment> _walls = new List<Raycast.Segment>(128);
    private List<Vector2> _polygonPoints = new List<Vector2>(128);

    private struct AnglePoint { public float Angle; public Vector2 Pos; }
    private List<AnglePoint> _sortedPoints = new List<AnglePoint>(128);

    /// <summary>
    /// Calculates the light polygon for a given light source.
    /// </summary>
    /// <param name="map">The world map.</param>
    /// <param name="lightPos">The position of the light source.</param>
    /// <param name="radius">The radius of the light source.</param>
    /// <returns>An array of points representing the vertices of the light polygon.</returns>
    public Vector2[] CalculateLightPolygon(WorldMap map, Vector2 lightPos, float radius)
    {
        _pointsOfInterest.Clear();
        _walls.Clear();
        _polygonPoints.Clear();
        _sortedPoints.Clear();

        int gridRad = (int)(radius / WorldMap.TILE_SIZE) + 1;
        var (cx, cy) = map.WorldToGrid(lightPos.X, lightPos.Y);

        for (int y = cy - gridRad; y <= cy + gridRad; y++)
        {
            for (int x = cx - gridRad; x <= cx + gridRad; x++)
            {
                if (x < 0 || x >= map.Width || y < 0 || y >= map.Height) continue;
                if (Vector2.Distance(lightPos, new Vector2(x * WorldMap.TILE_SIZE, y * WorldMap.TILE_SIZE)) > radius * 1.5f) continue;

                ref readonly var tile = ref map.GetTile(x, y);
                if (tile.WallId != 0)
                {
                    var def = TileRegistry.Get(tile.WallId);
                    if (def != null && def.IsOpaque)
                    {
                        AddWallSegments(x, y);
                    }
                }
            }
        }

        AddBoxSegments(lightPos - new Vector2(radius), lightPos + new Vector2(radius));

        foreach (var target in _pointsOfInterest)
        {
            Vector2 dir = Vector2.Normalize(target - lightPos);
            float angle = MathF.Atan2(dir.Y, dir.X);

            CastAtAngle(lightPos, angle - 0.001f);
            CastAtAngle(lightPos, angle);
            CastAtAngle(lightPos, angle + 0.001f);
        }

        _sortedPoints.Sort((a, b) => a.Angle.CompareTo(b.Angle));

        var result = new Vector2[_sortedPoints.Count + 1];
        result[0] = lightPos;
        for (int i = 0; i < _sortedPoints.Count; i++)
        {
            result[i + 1] = _sortedPoints[i].Pos;
        }

        return result;
    }

    private void CastAtAngle(Vector2 origin, float angle)
    {
        Vector2 dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        var ray = new Raycast.Ray { Origin = origin, Direction = dir };

        float minDist = float.MaxValue;
        Vector2 closestPoint = Vector2.Zero;
        bool hitAny = false;

        foreach (var wall in _walls)
        {
            var hit = Raycast.GetIntersection(ray, wall);
            if (hit.Hit && hit.Distance < minDist)
            {
                minDist = hit.Distance;
                closestPoint = hit.Point;
                hitAny = true;
            }
        }

        if (hitAny)
        {
            _sortedPoints.Add(new AnglePoint { Angle = angle, Pos = closestPoint });
        }
    }

    private void AddWallSegments(int x, int y)
    {
        float ts = WorldMap.TILE_SIZE;
        float wx = x * ts;
        float wy = y * ts;

        var p1 = new Vector2(wx, wy);
        var p2 = new Vector2(wx + ts, wy);
        var p3 = new Vector2(wx + ts, wy + ts);
        var p4 = new Vector2(wx, wy + ts);

        _pointsOfInterest.Add(p1);
        _pointsOfInterest.Add(p2);
        _pointsOfInterest.Add(p3);
        _pointsOfInterest.Add(p4);

        _walls.Add(new Raycast.Segment { A = p1, B = p2 });
        _walls.Add(new Raycast.Segment { A = p2, B = p3 });
        _walls.Add(new Raycast.Segment { A = p3, B = p4 });
        _walls.Add(new Raycast.Segment { A = p4, B = p1 });
    }

    private void AddBoxSegments(Vector2 min, Vector2 max)
    {
        var p1 = new Vector2(min.X, min.Y);
        var p2 = new Vector2(max.X, min.Y);
        var p3 = new Vector2(max.X, max.Y);
        var p4 = new Vector2(min.X, max.Y);

        _pointsOfInterest.Add(p1);
        _pointsOfInterest.Add(p2);
        _pointsOfInterest.Add(p3);
        _pointsOfInterest.Add(p4);

        _walls.Add(new Raycast.Segment { A = p1, B = p2 });
        _walls.Add(new Raycast.Segment { A = p2, B = p3 });
        _walls.Add(new Raycast.Segment { A = p3, B = p4 });
        _walls.Add(new Raycast.Segment { A = p4, B = p1 });
    }
}
