// Isotope.Core/Map/WorldMap.cs
using Isotope.Core.Tiles;

namespace Isotope.Core.Map;

public class WorldMap
{
    private readonly ushort[,] _tiles;
    public int Width { get; }
    public int Height { get; }

    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new ushort[width, height];
    }

    public ushort GetTileId(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return 0; // Return "air" for out-of-bounds tiles
        }
        return _tiles[x, y];
    }

    public void SetTileId(int x, int y, ushort id)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            _tiles[x, y] = id;
        }
    }

    public TileDefinition GetTileDef(int x, int y)
    {
        ushort id = GetTileId(x, y);
        return TileRegistry.Get(id);
    }
}
