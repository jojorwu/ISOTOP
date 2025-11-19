using Arch.Core;
using Isotope.Core.Prototypes;
using System;
using System.Numerics;

namespace Isotope.Core.Scripting;

/// <summary>
/// Provides a scripting API for interacting with the game engine.
/// </summary>
public class EngineApi
{
    private World _world;

    /// <summary>
    /// Initializes a new instance of the <see cref="EngineApi"/> class.
    /// </summary>
    /// <param name="world">The game world.</param>
    public EngineApi(World world)
    {
        _world = world;
    }

    /// <summary>
    /// Spawns an entity from a prototype.
    /// </summary>
    /// <param name="prototypeId">The ID of the prototype to spawn.</param>
    /// <param name="x">The x-coordinate to spawn the entity at.</param>
    /// <param name="y">The y-coordinate to spawn the entity at.</param>
    /// <returns>The ID of the spawned entity.</returns>
    public int Spawn(string prototypeId, float x, float y)
    {
        var entity = PrototypeManager.Spawn(_world, prototypeId, new Vector2(x, y));
        return entity.Id;
    }

    /// <summary>
    /// Logs a message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message) => Console.WriteLine($"[SCRIPT] {message}");
}
