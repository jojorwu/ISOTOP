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

    public void Update(in float deltaTime)
    {
        var query = new QueryDescription().WithAll<TransformComponent, SpriteComponent>();

        _world.Query(in query, (ref TransformComponent t, ref SpriteComponent s) =>
        {
             if (s.Texture.Id == 0) return;

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

}
