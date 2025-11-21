using ImGuiNET;
using Isotope.Core.Map;
using Isotope.Core.Prototypes;
using Raylib_cs;
using System.Numerics;
using Arch.Core;

namespace Isotope.Client.Editor;

public class EditorSystem
{
    private WorldMap _map;
    private World _world;
    private bool _isActive = false;
    private ushort _selectedTileId = 1;
    private int _layerMode = 0; // 0 = Floor, 1 = Wall, 2 = Objects
    private string _selectedEntity = "Toolbox";

    public EditorSystem(WorldMap map, World world)
    {
        _map = map;
        _world = world;
    }

    public void Update(Camera2D camera)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.F1)) _isActive = !_isActive;
        if (!_isActive) return;

        DrawGrid(camera);
        DrawToolbar();
        HandlePainting(camera);
    }

    private void DrawGrid(Camera2D cam)
    {
        Vector2 start = Raylib.GetScreenToWorld2D(Vector2.Zero, cam);
        Vector2 end = Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), cam);

        for (float x = start.X - (start.X % WorldMap.TILE_SIZE); x < end.X; x += WorldMap.TILE_SIZE)
            Raylib.DrawLineV(new Vector2(x, start.Y), new Vector2(x, end.Y), new Color(255, 255, 255, 50));

        for (float y = start.Y - (start.Y % WorldMap.TILE_SIZE); y < end.Y; y += WorldMap.TILE_SIZE)
            Raylib.DrawLineV(new Vector2(start.X, y), new Vector2(end.X, y), new Color(255, 255, 255, 50));
    }

    private void DrawToolbar()
    {
        ImGui.Begin("Map Editor");

        if (ImGui.CollapsingHeader("Tiles"))
        {
            if (ImGui.RadioButton("Floor", _layerMode == 0)) _layerMode = 0;
            ImGui.SameLine();
            if (ImGui.RadioButton("Wall", _layerMode == 1)) _layerMode = 1;

            var tiles = new[] { ("Plating", (ushort)1), ("Wall", (ushort)2), ("Erase", (ushort)0) };
            foreach (var t in tiles)
            {
                if (ImGui.Selectable(t.Item1, _selectedTileId == t.Item2)) _selectedTileId = t.Item2;
            }
        }

        if (ImGui.CollapsingHeader("Objects"))
        {
            if (ImGui.RadioButton("Spawn Objects", _layerMode == 2)) _layerMode = 2;

            string[] entities = { "Toolbox" }; // In future, get from PrototypeManager
            foreach (var e in entities)
            {
                if (ImGui.Selectable(e, _selectedEntity == e)) _selectedEntity = e;
            }
        }

        ImGui.Separator();
        if (ImGui.Button("Save Map")) MapSerializer.Save(_map, "level1.json");
        if (ImGui.Button("Load Map")) MapSerializer.Load(_map, "level1.json");

        ImGui.End();
    }

    private void HandlePainting(Camera2D camera)
    {
        if (ImGui.GetIO().WantCaptureMouse) return;

        Vector2 mouseWorld = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);

        if (_layerMode <= 1) // Tile painting
        {
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                var (gx, gy) = _map.WorldToGrid(mouseWorld.X, mouseWorld.Y);
                if (gx >= 0 && gx < _map.Width && gy >= 0 && gy < _map.Height)
                {
                    ref var tile = ref _map.GetTile(gx, gy);
                    if (_layerMode == 0) tile.FloorId = _selectedTileId;
                    else tile.WallId = _selectedTileId;
                }
            }
        }
        else // Object spawning
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                PrototypeManager.Spawn(_world, _selectedEntity, mouseWorld);
            }
        }
    }
}
