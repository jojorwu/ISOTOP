using Arch.Core;
using Isotope.Core.Components;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using YamlDotNet.Serialization;

namespace Isotope.Core.Prototypes;

public static class PrototypeManager
{
    private static Dictionary<string, EntityPrototype> _prototypes = new();

    public static void Register(EntityPrototype proto)
    {
        _prototypes[proto.Id] = proto;
    }

    public static Entity Spawn(World world, string protoId, Vector2 position)
    {
        if (!_prototypes.TryGetValue(protoId, out var proto))
        {
            Console.WriteLine($"[ERROR] Unknown prototype: {protoId}");
            return Entity.Null;
        }

        var entity = world.Create();
        world.Add(entity, new TransformComponent { LocalPosition = position });

        foreach (var compProto in proto.Components)
        {
            switch (compProto.Type)
            {
                case "Sprite":
                    var sprite = new SpriteComponent
                    {
                        TexturePath = GetValue<string>(compProto.Values, "texturePath"),
                        Tint = Color.White,
                        Visible = true
                    };
                    world.Add<SpriteComponent>(entity, sprite);
                    break;

                case "Body":
                    var body = new BodyComponent
                    {
                        Size = new Vector2(
                            GetValue<float>(compProto.Values, "width"),
                            GetValue<float>(compProto.Values, "height")
                        )
                    };
                    world.Add<BodyComponent>(entity, body);
                    break;

                case "LightSource":
                    var light = new LightSource
                    {
                        Radius = GetValue<float>(compProto.Values, "radius"),
                        Intensity = GetValue<float>(compProto.Values, "intensity"),
                        Color = new Color(
                            GetValue<int>(compProto.Values, "r"),
                            GetValue<int>(compProto.Values, "g"),
                            GetValue<int>(compProto.Values, "b"),
                            255
                        )
                    };
                    world.Add<LightSource>(entity, light);
                    break;

                case "PlayerTag":
                    world.Add<PlayerTag>(entity);
                    break;
            }
        }
        return entity;
    }

    private static T GetValue<T>(Dictionary<string, object> dict, string key)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        return default;
    }
}
