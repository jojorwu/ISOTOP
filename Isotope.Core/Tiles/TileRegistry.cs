namespace Isotope.Core.Tiles;

/// <summary>
/// A static registry for all tile definitions.
/// </summary>
public static class TileRegistry
{
    private static readonly TileDefinition[] _definitions = new TileDefinition[65535];
    private static ushort _nextId = 1; // 0 is reserved for "empty"

    /// <summary>
    /// Registers a new tile definition.
    /// </summary>
    /// <param name="def">The tile definition to register.</param>
    public static void Register(TileDefinition def)
    {
        def.Id = _nextId;
        _definitions[_nextId] = def;
        _nextId++;
    }

    /// <summary>
    /// Gets a tile definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the tile definition to get.</param>
    /// <returns>The tile definition, or null if it doesn't exist.</returns>
    public static TileDefinition Get(ushort id)
    {
        return _definitions[id];
    }
}
