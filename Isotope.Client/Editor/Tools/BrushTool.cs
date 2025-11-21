using Arch.Core;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;
using Isotope.Client.Editor.Commands;
using System.Collections.Generic;

namespace Isotope.Client.Editor.Tools
{
    public class BrushTool : IEditorTool
    {
        public string Name => "Brush (B)";
        private bool _isDragging = false;
        private List<IEditorCommand> _commands = new();

        public void OnActivate() { }
        public void OnDeactivate()
        {
            _isDragging = false;
            _commands.Clear();
        }

        public void OnMouseDown(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx)
        {
            _isDragging = true;
            _commands.Clear();

            int x = (int)mouseGridPos.X;
            int y = (int)mouseGridPos.Y;
            var command = new PlaceTileCommand(map, x, y, ctx.ActiveLayer, ctx.SelectedTileId);
            command.Execute(); // Execute immediately for responsiveness
            _commands.Add(command);
        }

        public void OnMouseMove(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx)
        {
            if (!_isDragging) return;

            int x = (int)mouseGridPos.X;
            int y = (int)mouseGridPos.Y;
            var command = new PlaceTileCommand(map, x, y, ctx.ActiveLayer, ctx.SelectedTileId);
            command.Execute();
            _commands.Add(command);
        }

        public void OnMouseUp(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx)
        {
            if (!_isDragging) return;

            _isDragging = false;
            history.Execute(new CompositeCommand(_commands));
        }

        public void DrawGizmos(WorldMap map, Vector2 mouseGridPos, EditorContext ctx)
        {
            Vector2 worldPos = mouseGridPos * WorldMap.TILE_SIZE;
            Raylib.DrawRectangleLines((int)worldPos.X, (int)worldPos.Y, 32, 32, Color.Green);
        }
    }
}
