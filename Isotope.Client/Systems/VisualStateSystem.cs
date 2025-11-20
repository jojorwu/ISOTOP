using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Components.Visuals;

namespace Isotope.Client.Systems
{
    public class VisualStateSystem
    {
        private World _world;

        public VisualStateSystem(World world)
        {
            _world = world;
        }

        public void Update(in float dt)
        {
            var query = new QueryDescription().WithAll<SpriteComponent, SpriteStateComponent>();

            _world.Query(in query, (ref SpriteComponent sprite, ref SpriteStateComponent state) =>
            {
                string newPath = $"{state.StateBase}_{state.CurrentState}.png";

                if (sprite.TexturePath != newPath)
                {
                    sprite.TexturePath = newPath;
                }
            });
        }
    }
}
