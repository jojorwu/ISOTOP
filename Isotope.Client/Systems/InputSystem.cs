using Arch.Core;
using Isotope.Core.Components;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Systems;

public partial class InputSystem
{
    private readonly World _world;
    private readonly float _speed = 200.0f;

    public InputSystem(World world)
    {
        _world = world;
    }

    public void Update(in float deltaTime)
    {
        var query = new QueryDescription().WithAll<PlayerTag, BodyComponent>();
        _world.Query(in query, (ref BodyComponent body) =>
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
