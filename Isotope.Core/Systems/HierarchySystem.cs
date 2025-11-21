using Arch.Core;
using Isotope.Core.Components;
using System.Collections.Generic;

namespace Isotope.Core.Systems;

public partial class HierarchySystem
{
    private readonly World _world;
    private readonly QueryDescription _allTransformsQuery = new QueryDescription().WithAll<TransformComponent>();

    public HierarchySystem(World world)
    {
        _world = world;
    }

    public void Update(in float deltaTime)
    {
        // First pass: update all root entities (those without a valid parent)
        _world.Query(in _allTransformsQuery, (Entity entity, ref TransformComponent transform) =>
        {
            if (!_world.IsAlive(transform.Parent))
            {
                transform.WorldPosition = transform.LocalPosition;
            }
        });

        // Subsequent passes to resolve children. This is simple and has limitations
        // but works for reasonably shallow hierarchies. A better system would use dirty flagging.
        for (int i = 0; i < 5; i++)
        {
            _world.Query(in _allTransformsQuery, (Entity entity, ref TransformComponent transform) =>
            {
                if (_world.IsAlive(transform.Parent))
                {
                    // This try-get is necessary because the parent might not have a TransformComponent,
                    // although in our design it always should.
                    if (_world.TryGet(transform.Parent, out TransformComponent parentTransform))
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
