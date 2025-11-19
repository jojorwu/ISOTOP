namespace Isotope.Core;

public class Map
{
    public readonly int Width;
    public readonly int Height;
    public readonly ushort[] Tiles;

    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new ushort[width * height];
    }

    public void GenerateSimpleTestMap(TileRegistry registry)
    {
        var floorId = registry.GetTile("floor").Id;
        var wallId = registry.GetTile("wall").Id;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int index = y * Width + x;
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                {
                    Tiles[index] = wallId;
                }
                else
                {
                    Tiles[index] = floorId;
                }
            }
        }
    }
}
