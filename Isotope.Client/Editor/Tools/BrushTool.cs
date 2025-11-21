using Arch.Core;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Editor.Tools
{
    public class BrushTool : IEditorTool
    {
        public string Name => "Brush (B)";
        private bool _isDragging = false;
        private System.Collections.Generic.List<Commands.IEditorCommand> _commands = new();

        public void OnUpdate(World world, WorldMap map, Vector2 mouseGridPos, Commands.HistoryManager history, EditorContext ctx)
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _isDragging = true;
                _commands.Clear();
            }

            if (_isDragging && Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                int x = (int)mouseGridPos.X;
                int y = (int)mouseGridPos.Y;
                _commands.Add(new Commands.PlaceTileCommand(map, x, y, ctx.ActiveLayer, ctx.SelectedTileId));
            }

            if (_isDragging && Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                _isDragging = false;
                history.Execute(new Commands.CompositeCommand(_commands));
            }
        }

        public void OnDrawGizmos(WorldMap map, Vector2 mouseGridPos)
        {
            Vector2 worldPos = mouseGridPos * WorldMap.TILE_SIZE;
            Raylib.DrawRectangleV(worldPos, new Vector2(32, 32), new Color(0, 255, 0, 100));
        }
    }
}
