using Arch.Core;
using Arch.System;
using Isotope.Core.Components;
using System.Collections.Generic;

namespace Isotope.Core.Systems;

/// <summary>
/// A system that manages the hierarchy of entities in the game world.
/// </summary>
public partial class HierarchySystem : BaseSystem<World, float>
{
    private readonly QueryDescription _allTransformsQuery = new QueryDescription().WithAll<TransformComponent>();

    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchySystem"/> class.
    /// </summary>
    /// <param name="world">The game world.</param>
    public HierarchySystem(World world) : base(world) { }

    /// <summary>
    /// Updates the system, resolving the world positions of all entities in the hierarchy.
    /// </summary>
    /// <param name="deltaTime">The time since the last update.</param>
    public override void Update(in float deltaTime)
    {
        // First pass: update all root entities (those without a valid parent)
        World.Query(in _allTransformsQuery, (Entity entity, ref TransformComponent transform) =>
        {
            if (!transform.Parent.IsAlive() || !World.IsAlive(transform.Parent))
            {
                transform.WorldPosition = transform.LocalPosition;
            }
        });

        // Subsequent passes to resolve children. This is simple and has limitations
        // but works for reasonably shallow hierarchies. A better system would use dirty flagging.
        for (int i = 0; i < 5; i++)
        {
            World.Query(in _allTransformsQuery, (Entity entity, ref TransformComponent transform) =>
            {
                if (transform.Parent.IsAlive() && World.IsAlive(transform.Parent))
                {
                    // This try-get is necessary because the parent might not have a TransformComponent,
                    // although in our design it always should.
                    if (World.TryGet(transform.Parent, out TransformComponent parentTransform))
                    {
                        transform.WorldPosition = parentTransform.WorldPosition + transform.LocalPosition;
                    }
                    else // If parent is invalid, detach this entity
                    {
                        transform.Parent = Entity.Null;
                        transform.WorldPosition = transform.LocalPosition;
                    }
                }
            });
        }
    }
}
