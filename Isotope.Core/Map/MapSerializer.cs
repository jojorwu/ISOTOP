using Isotope.Core.Map;
using System;
using System.IO;
using System.Text.Json;
/// <summary>
/// Provides functionality for saving and loading map data.
/// </summary>
public static class MapSerializer
{
    /// <summary>
    /// Represents the data structure for serializing a map.
    /// </summary>
    public class MapData
    {
        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The array of floor tiles.
        /// </summary>
        public ushort[] Floors { get; set; }
        /// <summary>
        /// The array of wall tiles.
        /// </summary>
        public ushort[] Walls { get; set; }
    }

    /// <summary>
    /// Saves the specified map to a file.
    /// </summary>
    /// <param name="map">The map to save.</param>
    /// <param name="filename">The name of the file to save the map to.</param>
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

    /// <summary>
    /// Loads a map from the specified file.
    /// </summary>
    /// <param name="map">The map to load the data into.</param>
    /// <param name="filename">The name of the file to load the map from.</param>
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
