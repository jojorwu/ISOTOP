/// <summary>
/// Represents the definition of a tile, including its properties and appearance.
/// </summary>
public class TileDefinition
{
    /// <summary>
    /// The unique identifier of the tile.
    /// </summary>
    public ushort Id { get; internal set; }

    /// <summary>
    /// The name of the tile, e.g., "steel_wall" or "grass".
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The path to the tile's sprite.
    /// </summary>
    public string TexturePath { get; set; }

    /// <summary>
    /// A value indicating whether the tile is solid and cannot be passed through.
    /// </summary>
    public bool IsSolid { get; set; } = false;

    /// <summary>
    /// A value indicating whether the tile is opaque and blocks light.
    /// </summary>
    public bool IsOpaque { get; set; } = false;

    /// <summary>
    /// The friction of the tile, where values less than 1.0 represent slippery surfaces like ice.
    /// </summary>
    public float Friction { get; set; } = 1.0f;
}
