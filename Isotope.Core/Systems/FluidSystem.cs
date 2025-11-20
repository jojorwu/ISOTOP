using Arch.Core;
using Isotope.Core.Chemistry;
using Isotope.Core.Components;
using System.Numerics;

namespace Isotope.Core.Systems
{
    public class FluidSystem
    {
        public void Spill(World world, Vector2 pos, SolutionComponent solution)
        {
            var puddle = world.Create();

            world.Add(puddle, new TransformComponent { Position = pos });
            world.Add(puddle, new SpriteComponent { TexturePath = "fluids/puddle_mask", Tint = solution.GetColor() });

            world.Add(puddle, solution);
        }
    }
}
