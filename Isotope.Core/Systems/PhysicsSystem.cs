using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Map;
using Isotope.Core.Tiles;
using System.Numerics;

namespace Isotope.Core.Systems;

public partial class PhysicsSystem
{
    private readonly World _world;
    private readonly WorldMap _map;
    private readonly QueryDescription _query = new QueryDescription().WithAll<TransformComponent, BodyComponent>();

    public PhysicsSystem(World world, WorldMap map)
    {
        _world = world;
        _map = map;
    }

    public void Update(in float deltaTime)
    {
        var dt = deltaTime;
        _world.Query(in _query, (ref TransformComponent pos, ref BodyComponent body) =>
        {
            if (body.IsStatic) return;

            // Physics operates on WorldPosition
            Vector2 nextPos = pos.WorldPosition + (body.Velocity * dt);

            if (!IsCollidingWithMap(nextPos, body.Size))
            {
                // Update LocalPosition based on the change in WorldPosition
                pos.LocalPosition += nextPos - pos.WorldPosition;
            }
            else
            {
                body.Velocity = Vector2.Zero;
            }

            body.Velocity *= 0.90f;
        });
    }

    private bool IsCollidingWithMap(Vector2 pos, Vector2 size)
    {
        if (IsSolid(pos.X, pos.Y)) return true;
        if (IsSolid(pos.X + size.X, pos.Y)) return true;
        if (IsSolid(pos.X, pos.Y + size.Y)) return true;
        if (IsSolid(pos.X + size.X, pos.Y + size.Y)) return true;

        return false;
    }

    private bool IsSolid(float x, float y)
    {
        var (gridX, gridY) = _map.WorldToGrid(x, y);

        var tileDef = _map.GetTileDefForPhysics(gridX, gridY);
        return tileDef != null && tileDef.IsSolid;
    }
}
