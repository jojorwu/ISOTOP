using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Components.Objects;
using Isotope.Core.Components.Visuals;

namespace Isotope.Core.Systems
{
    public class DoorSystem
    {
        private World _world;

        public DoorSystem(World world)
        {
            _world = world;
        }

        public void Update(in float dt)
        {
            var query = new QueryDescription().WithAll<DoorComponent, SpriteStateComponent>();

            _world.Query(in query, (ref DoorComponent door, ref SpriteStateComponent visual) =>
            {
                if (door.StateTimer > 0)
                {
                    door.StateTimer -= dt;
                    if (door.StateTimer <= 0)
                    {
                        visual.CurrentState = door.IsOpen ? "open" : "closed";
                    }
                }
            });
        }

        public void ToggleDoor(Entity e)
        {
            ref var door = ref _world.Get<DoorComponent>(e);
            ref var visual = ref _world.Get<SpriteStateComponent>(e);

            if (door.IsLocked)
            {
                visual.CurrentState = "denied";
                return;
            }

            door.IsOpen = !door.IsOpen;
            door.StateTimer = door.OpenSpeed;

            visual.CurrentState = door.IsOpen ? "opening" : "closing";
        }
    }
}
