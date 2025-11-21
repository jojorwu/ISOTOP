using Arch.Core;
using Isotope.Core.Components;
using Raylib_cs;

namespace Isotope.Core.Systems;

/// <summary>
/// This system detects user interactions (like mouse clicks) with entities.
/// </summary>
public class InteractionSystem
{
    private readonly World _world;
    private readonly ScriptSystem _scriptSystem;
    private readonly Camera2D _camera;

    public InteractionSystem(World world, ScriptSystem scriptSystem, Camera2D camera)
    {
        _world = world;
        _scriptSystem = scriptSystem;
        _camera = camera;
    }

    public void Update()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            var mousePos = Raylib.GetMousePosition();
            var worldPos = Raylib.GetScreenToWorld2D(mousePos, _camera);

            // This is a simplified entity lookup. A real implementation would use a spatial hash or quadtree.
            var query = new QueryDescription().WithAll<Components.TransformComponent, Components.SpriteComponent>();
            _world.Query(in query, (Entity entity, ref Components.TransformComponent transform, ref Components.SpriteComponent sprite) =>
            {
                if (sprite.Texture.Id == 0) return; // Texture not loaded

                var bounds = new Rectangle(transform.Position.X, transform.Position.Y, sprite.Texture.Width, sprite.Texture.Height);
                if (Raylib.CheckCollisionPointRec(worldPos, bounds))
                {
                    _scriptSystem.OnInteract(entity);
                }
            });
        }
    }
}
