using System;
using Arch.Core;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Editor.Tools
{
    public class RectTool : IEditorTool
    {
        public string Name => "Rectangle (R)";

        private Vector2 _startPos = new Vector2(-1, -1);
        private bool _isDragging = false;

        public void OnUpdate(World world, WorldMap map, Vector2 mouseGridPos, Commands.HistoryManager history, EditorContext ctx)
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _startPos = mouseGridPos;
                _isDragging = true;
            }

            if (_isDragging && Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                ApplyRect(map, _startPos, mouseGridPos, history, ctx);
                _isDragging = false;
                _startPos = new Vector2(-1, -1);
            }
        }

        public void OnDrawGizmos(WorldMap map, Vector2 mouseGridPos)
        {
            if (_isDragging)
            {
                var rect = GetRect(_startPos, mouseGridPos);
                Raylib.DrawRectangleLinesEx(rect, 3, Color.Blue);
                Raylib.DrawRectangleRec(rect, new Color(0, 0, 255, 50));
            }
        }

        private void ApplyRect(WorldMap map, Vector2 start, Vector2 end, Commands.HistoryManager history, EditorContext ctx)
        {
            var commands = new System.Collections.Generic.List<Commands.IEditorCommand>();
            int x1 = (int)Math.Min(start.X, end.X);
            int y1 = (int)Math.Min(start.Y, end.Y);
            int x2 = (int)Math.Max(start.X, end.X);
            int y2 = (int)Math.Max(start.Y, end.Y);

            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    commands.Add(new Commands.PlaceTileCommand(map, x, y, ctx.ActiveLayer, ctx.SelectedTileId));
                }
            }
            history.Execute(new Commands.CompositeCommand(commands));
        }

        private Rectangle GetRect(Vector2 start, Vector2 end)
        {
            float x = Math.Min(start.X, end.X) * WorldMap.TILE_SIZE;
            float y = Math.Min(start.Y, end.Y) * WorldMap.TILE_SIZE;
            float w = (Math.Abs(end.X - start.X) + 1) * WorldMap.TILE_SIZE;
            float h = (Math.Abs(end.Y - start.Y) + 1) * WorldMap.TILE_SIZE;
            return new Rectangle(x, y, w, h);
        }
    }
}
