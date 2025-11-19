namespace Isotope.Core.Map;

/// <summary>
/// Represents a single tile in the game world, consisting of a floor and a wall.
/// </summary>
public struct MapTile
{
    /// <summary>
    /// The ID of the floor tile.
    /// </summary>
    public ushort FloorId;

    /// <summary>
    /// The ID of the wall tile.
    /// </summary>
    public ushort WallId;
}
