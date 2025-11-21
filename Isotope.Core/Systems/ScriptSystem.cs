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
    private readonly Lua _lua; // Single global Lua state
    private readonly Dictionary<Entity, LuaTable> _entityTables = new();
    private readonly Dictionary<Entity, LuaFunction> _updateFunctions = new();

    public ScriptSystem(World world, object engineApi)
    {
        _world = world;
        _engineApi = engineApi;
        _lua = new Lua();
        _lua.LoadCLRPackage();
        _lua["Game"] = _engineApi;
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

            if (_updateFunctions.TryGetValue(entity, out var updateFunction))
            {
                try
                {
                    // Pass the entity's table as the first argument ('self')
                    updateFunction.Call(_entityTables[entity], dt);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ScriptSystem ERROR] OnUpdate for Entity {entity.Id} ({script.ScriptPath}): {e.Message}");
                }
            }
        });

        // Clean up Lua instances for entities that no longer have a ScriptComponent
        var allWithInstances = new List<Entity>(_entityTables.Keys);
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
        if (_world.Has<ScriptComponent>(target) && _entityTables.TryGetValue(target, out var entityTable))
        {
            try
            {
                if (entityTable["OnInteract"] is LuaFunction interactFunction)
                {
                    interactFunction.Call(entityTable); // Pass the instance table as 'self'
                }
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
            // Execute the script, which should return a table (our script's "class")
            var result = _lua.DoFile(script.ScriptPath);
            if (result == null || result.Length == 0 || !(result[0] is LuaTable scriptClass))
            {
                Console.WriteLine($"[ScriptSystem ERROR] Script {script.ScriptPath} did not return a table.");
                script.IsInitialized = true;
                return;
            }

            // Create the instance table for this entity
            var entityTable = _lua.NewTable($"entity_instance_{entity.Id}");
            entityTable["EntityId"] = entity.Id;

            // Set up the metatable for inheritance.
            // This makes the instance table fall back to the class table for methods.
            var metatable = _lua.NewTable("metatable");
            metatable["__index"] = scriptClass;
            entityTable.Metatable = metatable;

            _entityTables[entity] = entityTable;

            // Call the OnCreate method on the new instance
            if (entityTable["OnCreate"] is LuaFunction createFunction)
            {
                createFunction.Call(entityTable); // Pass the instance table as 'self'
            }

            // Cache the update function for this instance
            if (entityTable["OnUpdate"] is LuaFunction updateFunction)
            {
                _updateFunctions[entity] = updateFunction;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ScriptSystem ERROR] Failed to load script {script.ScriptPath}: {e.Message}");
        }

        script.IsInitialized = true;
    }

    private void CleanupScript(Entity entity)
    {
        if (_entityTables.Remove(entity, out var table))
        {
            table.Dispose(); // Dispose the table to release resources in NLua
        }
        _updateFunctions.Remove(entity);
        // No need for DoString, the GC will handle the table if it's not referenced.
    }
}
