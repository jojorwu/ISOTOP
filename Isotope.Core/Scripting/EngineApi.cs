using Arch.Core;
using Isotope.Core.Prototypes;
using System;
using System.Numerics;

namespace Isotope.Core.Scripting;

public class EngineApi
{
    private World _world;

    public EngineApi(World world)
    {
        _world = world;
    }

    public int Spawn(string prototypeId, float x, float y)
    {
        var entity = PrototypeManager.Spawn(_world, prototypeId, new Vector2(x, y));
        return entity.Id;
    }

    public void Log(string message) => Console.WriteLine($"[SCRIPT] {message}");
}
