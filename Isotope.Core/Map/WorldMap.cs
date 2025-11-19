using Isotope.Core.Tiles;
using System.Numerics;

namespace Isotope.Core.Map;

/// <summary>
/// Represents the game world, composed of a grid of tiles.
/// </summary>
public class WorldMap
{
    /// <summary>
    /// The size of each tile in pixels.
    /// </summary>
    public const int TILE_SIZE = 32;
    private readonly MapTile[] _tiles;

    /// <summary>
    /// The width of the map in tiles.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of the map in tiles.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorldMap"/> class.
    /// </summary>
    /// <param name="width">The width of the map in tiles.</param>
    /// <param name="height">The height of the map in tiles.</param>
    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new MapTile[width * height];
    }

    /// <summary>
    /// Gets a reference to the tile at the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the tile.</param>
    /// <param name="y">The y-coordinate of the tile.</param>
    /// <returns>A reference to the <see cref="MapTile"/> at the specified coordinates.</returns>
    public ref MapTile GetTile(int x, int y)
    {
        return ref _tiles[y * Width + x];
    }

    /// <summary>
    /// Gets the raw array of tiles.
    /// </summary>
    /// <returns>The array of <see cref="MapTile"/> objects.</returns>
    public MapTile[] GetRawTiles()
    {
        return _tiles;
    }

    /// <summary>
    /// Gets the tile definition for physics calculations at the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the tile.</param>
    /// <param name="y">The y-coordinate of the tile.</param>
    /// <returns>The <see cref="TileDefinition"/> for the tile at the specified coordinates.</returns>
    public TileDefinition GetTileDefForPhysics(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;

        ref readonly var tile = ref GetTile(x,y);

        if (tile.WallId != 0)
        {
            return TileRegistry.Get(tile.WallId);
        }

        return TileRegistry.Get(tile.FloorId);
    }

    /// <summary>
    /// Converts world coordinates to grid coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate in the world.</param>
    /// <param name="y">The y-coordinate in the world.</param>
    /// <returns>A tuple containing the grid coordinates.</returns>
    public (int, int) WorldToGrid(float x, float y)
    {
        return ((int)(x / TILE_SIZE), (int)(y / TILE_SIZE));
    }
}
