using Arch.Core;
using Isotope.Core.Components;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;

namespace Isotope.Core.Systems;

/// <summary>
/// Manages and executes Lua scripts attached to entities via <see cref="ScriptComponent"/>.
/// </summary>
public class ScriptSystem
{
    private readonly World _world;
    private readonly object _engineApi; // API object exposed to Lua
    private readonly Dictionary<Entity, Lua> _luaInstances = new();

    public ScriptSystem(World world, object engineApi)
    {
        _world = world;
        _engineApi = engineApi;
    }

    public void Update(in float dt)
    {
        var query = new QueryDescription().WithAll<ScriptComponent>();

        // Initialize new scripts and call their OnUpdate function
        _world.Query(in query, (Entity entity, ref ScriptComponent script) =>
        {
            if (!script.IsInitialized)
            {
                InitializeScript(entity, ref script);
            }

            if (_luaInstances.TryGetValue(entity, out var lua))
            {
                try
                {
                    var updateFunction = lua["OnUpdate"] as LuaFunction;
                    updateFunction?.Call(dt);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ScriptSystem ERROR] OnUpdate for Entity {entity.Id} ({script.ScriptPath}): {e.Message}");
                }
            }
        });

        // Clean up Lua instances for entities that no longer have a ScriptComponent
        var allWithInstances = new List<Entity>(_luaInstances.Keys);
        foreach(var entity in allWithInstances)
        {
            if(!_world.IsAlive(entity) || !_world.Has<ScriptComponent>(entity))
            {
                CleanupScript(entity);
            }
        }
    }

    /// <summary>
    /// Called from other systems (e.g., InteractionSystem) to trigger an interaction event.
    /// </summary>
    public void OnInteract(Entity target)
    {
        if (_world.Has<ScriptComponent>(target) && _luaInstances.TryGetValue(target, out var lua))
        {
            try
            {
                var interactFunction = lua["OnInteract"] as LuaFunction;
                interactFunction?.Call();
            }
            catch (Exception e)
            {
                ref var script = ref _world.Get<ScriptComponent>(target);
                Console.WriteLine($"[ScriptSystem ERROR] OnInteract for Entity {target.Id} ({script.ScriptPath}): {e.Message}");
            }
        }
    }

    private void InitializeScript(Entity entity, ref ScriptComponent script)
    {
        if (!File.Exists(script.ScriptPath))
        {
            Console.WriteLine($"[ScriptSystem ERROR] Script file not found: {script.ScriptPath}");
            script.IsInitialized = true; // Mark as initialized to prevent repeated file checks
            return;
        }

        try
        {
            var lua = new Lua();
            lua.LoadCLRPackage();
            lua["Game"] = _engineApi;
            lua["EntityId"] = entity.Id;

            lua.DoFile(script.ScriptPath);
            _luaInstances[entity] = lua;

            var createFunction = lua["OnCreate"] as LuaFunction;
            createFunction?.Call();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ScriptSystem ERROR] Failed to load script {script.ScriptPath}: {e.Message}");
        }

        script.IsInitialized = true;
    }

    private void CleanupScript(Entity entity)
    {
        if (_luaInstances.TryGetValue(entity, out var lua))
        {
            lua.Dispose();
            _luaInstances.Remove(entity);
        }
    }
}
