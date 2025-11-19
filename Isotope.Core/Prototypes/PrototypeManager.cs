using Arch.Core;
using Isotope.Core.Components;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using YamlDotNet.Serialization;

namespace Isotope.Core.Prototypes;

/// <summary>
/// Manages entity prototypes and their creation.
/// </summary>
public static class PrototypeManager
{
    private static Dictionary<string, EntityPrototype> _prototypes = new();

    /// <summary>
    /// Registers an entity prototype.
    /// </summary>
    /// <param name="proto">The prototype to register.</param>
    public static void Register(EntityPrototype proto)
    {
        _prototypes[proto.Id] = proto;
    }

    /// <summary>
    /// Spawns an entity from a prototype.
    /// </summary>
    /// <param name="world">The world to spawn the entity in.</param>
    /// <param name="protoId">The ID of the prototype to spawn.</param>
    /// <param name="position">The position to spawn the entity at.</param>
    /// <returns>The spawned entity.</returns>
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
                    entity.Add(sprite);
                    break;

                case "Body":
                    var body = new BodyComponent
                    {
                        Size = new Vector2(
                            GetValue<float>(compProto.Values, "width"),
                            GetValue<float>(compProto.Values, "height")
                        )
                    };
                    entity.Add(body);
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
                    entity.Add(light);
                    break;

                case "PlayerTag":
                    entity.Add(new PlayerTag());
                    break;
            }
        }
        return entity;
    }

    /// <summary>
    /// Gets a value from a dictionary, converting it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="dict">The dictionary to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value, or the default value of the type if the key is not found.</returns>
    private static T GetValue<T>(Dictionary<string, object> dict, string key)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        return default;
    }
}
