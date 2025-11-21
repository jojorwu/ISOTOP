using Arch.Core;
using Isotope.Core.Components;
using Raylib_cs;
using System.Collections.Generic;

namespace Isotope.Client.Systems;

public partial class EntityRenderSystem
{
    private readonly World _world;

    public EntityRenderSystem(World world)
    {
        _world = world;
    }

    public void Update(in Camera2D camera)
    {
        var cameraBounds = GetCameraWorldBounds(camera);
        var query = new QueryDescription().WithAll<TransformComponent, SpriteComponent>().WithNone<PlayerTag>();

        _world.Query(in query, (ref TransformComponent t, ref SpriteComponent s) =>
        {
            if (s.Texture.Id == 0) return;

            // Basic culling logic
            var entityBounds = new Rectangle(t.WorldPosition.X - s.Texture.Width / 2, t.WorldPosition.Y - s.Texture.Height / 2, s.Texture.Width, s.Texture.Height);
            if (!Raylib.CheckCollisionRecs(cameraBounds, entityBounds))
            {
                return;
            }

             var texture = s.Texture;

             if (s.SourceRect.Width > 0 && s.SourceRect.Height > 0)
             {
                 var destRect = new Rectangle(t.WorldPosition.X, t.WorldPosition.Y, s.SourceRect.Width, s.SourceRect.Height);
                 Raylib.DrawTexturePro(texture, s.SourceRect, destRect, new System.Numerics.Vector2(s.SourceRect.Width / 2, s.SourceRect.Height / 2), t.Rotation, s.Tint);
             }
             else
             {
                 Raylib.DrawTextureEx(texture, t.WorldPosition, t.Rotation, 1.0f, s.Tint);
             }
        });
    }

    private Rectangle GetCameraWorldBounds(Camera2D camera)
    {
        var topLeft = Raylib.GetScreenToWorld2D(new System.Numerics.Vector2(0, 0), camera);
        var bottomRight = Raylib.GetScreenToWorld2D(new System.Numerics.Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), camera);
        return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }
}
