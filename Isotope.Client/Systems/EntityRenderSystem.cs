using Arch.Core;
using Isotope.Core.Components;
using Raylib_cs;
using System.Collections.Generic;

namespace Isotope.Client.Systems;

public partial class EntityRenderSystem
{
    private readonly World _world;
    private Dictionary<string, Texture2D> _textureCache = new();

    public EntityRenderSystem(World world)
    {
        _world = world;
    }

    public void Update(in float deltaTime)
    {
        var query = new QueryDescription().WithAll<TransformComponent, SpriteComponent>();

        _world.Query(in query, (ref TransformComponent t, ref SpriteComponent s) =>
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
