using Isotope.Core.Map;
using System;
using System.IO;
using System.Text.Json;

public static class MapSerializer
{
    public class MapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ushort[] Floors { get; set; }
        public ushort[] Walls { get; set; }
    }

    public static void Save(WorldMap map, string filename)
    {
        var data = new MapData
        {
            Width = map.Width,
            Height = map.Height,
            Floors = new ushort[map.Width * map.Height],
            Walls = new ushort[map.Width * map.Height]
        };

        var rawTiles = map.GetRawTiles();
        for (int i = 0; i < map.Width * map.Height; i++)
        {
            data.Floors[i] = rawTiles[i].FloorId;
            data.Walls[i] = rawTiles[i].WallId;
        }

        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filename, json);
        Console.WriteLine($"Map saved to {filename}");
    }

    public static void Load(WorldMap map, string filename)
    {
        if (!File.Exists(filename)) return;

        string json = File.ReadAllText(filename);
        var data = JsonSerializer.Deserialize<MapData>(json);

        if (data.Width != map.Width || data.Height != map.Height)
        {
            Console.WriteLine("[ERROR] Map dimensions do not match!");
            return;
        }

        var rawTiles = map.GetRawTiles();
        for (int i = 0; i < data.Floors.Length; i++)
        {
            rawTiles[i].FloorId = data.Floors[i];
            rawTiles[i].WallId = data.Walls[i];
        }
        Console.WriteLine("Map loaded!");
    }
}
