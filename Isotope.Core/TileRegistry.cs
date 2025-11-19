using System.Collections.Generic;

namespace Isotope.Core;

public class TileRegistry
{
    public readonly Dictionary<string, TileDefinition> Definitions = new();

    public void RegisterTiles()
    {
        // For now, we hardcode the tiles. Later, this will be loaded from JSON.
        var floor = new TileDefinition
        {
            Id = 1,
            InternalName = "floor",
            TexturePath = "textures/floor.png",
            IsSolid = false,
            ThermalConductivity = 0.1f
        };

        var wall = new TileDefinition
        {
            Id = 2,
            InternalName = "wall",
            TexturePath = "textures/wall.png",
            IsSolid = true,
            ThermalConductivity = 0.5f
        };

        Definitions.Add(floor.InternalName, floor);
        Definitions.Add(wall.InternalName, wall);
    }

    public TileDefinition GetTile(string internalName)
    {
        return Definitions[internalName];
    }

    public TileDefinition GetTile(ushort id)
    {
        // This is inefficient and will be optimized later.
        foreach (var def in Definitions.Values)
        {
            if (def.Id == id) return def;
        }
        return null;
    }
}
