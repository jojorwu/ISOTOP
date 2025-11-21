using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Prototypes;
using System;
using System.Numerics;

namespace Isotope.Core.Scripting;

/// <summary>
/// A "flat" API exposed to Lua scripts to interact with the game world.
/// This API is designed to be performant by avoiding the marshaling of complex C# objects.
/// All methods should operate on entity IDs and primitive types.
/// </summary>
public class EngineApi
{
    private readonly World _world;

    public EngineApi(World world)
    {
        _world = world;
    }

    #region Spawning
    public int Spawn(string prototypeId, float x, float y)
    {
        var entity = PrototypeManager.Spawn(_world, prototypeId, new Vector2(x, y));
        return entity.Id;
    }
    #endregion

    #region Logging
    public void Log(string message) => Console.WriteLine($"[SCRIPT] {message}");
    #endregion

    #region TransformComponent Access
    public Vector2 GetPosition(int entityId)
    {
        var entity = new Entity(entityId, _world.Id);
        if (_world.IsAlive(entity) && _world.Has<TransformComponent>(entity))
        {
            return _world.Get<TransformComponent>(entity).Position;
        }
        return Vector2.Zero;
    }

    public void SetPosition(int entityId, float x, float y)
    {
        var entity = new Entity(entityId, _world.Id);
        if (_world.IsAlive(entity) && _world.Has<TransformComponent>(entity))
        {
            ref var transform = ref _world.Get<TransformComponent>(entity);
            transform.Position = new Vector2(x, y);
        }
    }
    #endregion

    #region PhysicsComponent Access
    public Vector2 GetVelocity(int entityId)
    {
        var entity = new Entity(entityId, _world.Id);
        if (_world.IsAlive(entity) && _world.Has<PhysicsComponent>(entity))
        {
            return _world.Get<PhysicsComponent>(entity).Velocity;
        }
        return Vector2.Zero;
    }

    public void SetVelocity(int entityId, float x, float y)
    {
        var entity = new Entity(entityId, _world.Id);
        if (_world.IsAlive(entity) && _world.Has<PhysicsComponent>(entity))
        {
            ref var physics = ref _world.Get<PhysicsComponent>(entity);
            physics.Velocity = new Vector2(x, y);
        }
    }
    #endregion

    #region Component Check
    public bool HasComponent(int entityId, string componentName)
    {
        var entity = new Entity(entityId, _world.Id);
        if (!_world.IsAlive(entity)) return false;

        // This is a bit slow due to reflection, but it's the cleanest way
        // to check for components from Lua without a massive switch statement.
        // It should be used sparingly, primarily in initialization logic.
        Type componentType = Type.GetType($"Isotope.Core.Components.{componentName}");
        if (componentType != null)
        {
            return (bool)typeof(World).GetMethod("Has").MakeGenericMethod(componentType).Invoke(_world, new object[] { entity });
        }
        return false;
    }
    #endregion
}
