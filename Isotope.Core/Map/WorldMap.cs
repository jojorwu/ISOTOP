using Isotope.Core.Tiles;
using System.Numerics;

namespace Isotope.Core.Map;

public class WorldMap
{
    public const int TILE_SIZE = 32;
    private readonly MapTile[] _tiles;
    public int Width { get; }
    public int Height { get; }

    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new MapTile[width * height];
    }

    public ref MapTile GetTile(int x, int y)
    {
        return ref _tiles[y * Width + x];
    }

    public MapTile[] GetRawTiles()
    {
        return _tiles;
    }

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

    public (int, int) WorldToGrid(float x, float y)
    {
        return ((int)(x / TILE_SIZE), (int)(y / TILE_SIZE));
    }
}
