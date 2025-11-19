using Arch.Core;
using Arch.System;
using Isotope.Core.Components;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Systems;

/// <summary>
/// A system that handles player input.
/// </summary>
public partial class InputSystem : BaseSystem<World, float>
{
    private readonly float _speed = 200.0f;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputSystem"/> class.
    /// </summary>
    /// <param name="world">The game world.</param>
    public InputSystem(World world) : base(world) { }

    /// <summary>
    /// Updates the system, handling player input.
    /// </summary>
    /// <param name="deltaTime">The time since the last update.</param>
    public override void Update(in float deltaTime)
    {
        var query = new QueryDescription().WithAll<PlayerTag, BodyComponent>();
        World.Query(in query, (ref BodyComponent body) =>
        {
            Vector2 direction = Vector2.Zero;
            if (Raylib.IsKeyDown(KeyboardKey.W)) direction.Y -= 1;
            if (Raylib.IsKeyDown(KeyboardKey.S)) direction.Y += 1;
            if (Raylib.IsKeyDown(KeyboardKey.A)) direction.X -= 1;
            if (Raylib.IsKeyDown(KeyboardKey.D)) direction.X += 1;

            if (direction != Vector2.Zero)
            {
                body.Velocity = Vector2.Normalize(direction) * _speed;
            }
        });
    }
}
