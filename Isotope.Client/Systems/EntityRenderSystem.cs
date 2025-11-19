using Arch.Core;
using Arch.System;
using Isotope.Core.Components;
using Raylib_cs;
using System.Collections.Generic;

namespace Isotope.Client.Systems;

public partial class EntityRenderSystem : BaseSystem<World, float>
{
    private Dictionary<string, Texture2D> _textureCache = new();

    public EntityRenderSystem(World world) : base(world) { }

    public override void Update(in float deltaTime)
    {
        var query = new QueryDescription().WithAll<TransformComponent, SpriteComponent>();

        World.Query(in query, (ref TransformComponent t, ref SpriteComponent s) =>
        {
             if (!s.Visible) return;

             var texture = GetTexture(s.TexturePath);

             Raylib.DrawTextureEx(texture, t.WorldPosition, t.Rotation, 1.0f, s.Tint);
        });
    }

    private Texture2D GetTexture(string path)
    {
        if (_textureCache.TryGetValue(path, out var texture))
        {
            return texture;
        }

        var newTexture = Raylib.LoadTexture(path);
        _textureCache[path] = newTexture;
        return newTexture;
    }
}
