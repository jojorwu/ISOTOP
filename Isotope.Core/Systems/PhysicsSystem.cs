using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Map;
using Isotope.Core.Tiles;
using System.Numerics;

namespace Isotope.Core.Systems;

/// <summary>
/// A system that handles physics and collision for entities.
/// </summary>
public partial class PhysicsSystem : ParallelSystem
{
    private readonly WorldMap _map;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicsSystem"/> class.
    /// </summary>
    /// <param name="world">The game world.</param>
    /// <param name="map">The world map.</param>
    public PhysicsSystem(World world, WorldMap map)
        : base(world, new QueryDescription().WithAll<TransformComponent, BodyComponent>())
    {
        _map = map;
    }

    /// <summary>
    /// Updates a chunk of entities, applying physics and collision.
    /// </summary>
    /// <param name="chunk">The chunk to update.</param>
    /// <param name="deltaTime">The time since the last update.</param>
    protected override void UpdateChunk(ref Chunk chunk, in float deltaTime)
    {
        var positions = chunk.GetSpan<TransformComponent>();
        var bodies = chunk.GetSpan<BodyComponent>();

        for (int i = 0; i < chunk.Size; i++)
        {
            ref var pos = ref positions[i];
            ref var body = ref bodies[i];

            if (body.IsStatic) continue;

            // Physics operates on WorldPosition
            Vector2 nextPos = pos.WorldPosition + (body.Velocity * deltaTime);

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
        }
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
