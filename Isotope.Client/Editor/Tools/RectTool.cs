using System;
using Arch.Core;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;
using Isotope.Client.Editor.Commands;
using System.Collections.Generic;

namespace Isotope.Client.Editor.Tools
{
    public class RectTool : IEditorTool
    {
        public string Name => "Rectangle (R)";

        private Vector2 _startPos = new Vector2(-1, -1);
        private bool _isDragging = false;

        public void OnActivate() { }
        public void OnDeactivate()
        {
            _isDragging = false;
            _startPos = new Vector2(-1, -1);
        }

        public void OnMouseDown(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx)
        {
            _isDragging = true;
            _startPos = mouseGridPos;
        }

        public void OnMouseMove(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx)
        {
            // Mouse move logic can be added here if needed for previews, etc.
        }

        public void OnMouseUp(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx)
        {
            if (!_isDragging) return;

            ApplyRect(map, _startPos, mouseGridPos, history, ctx);
            _isDragging = false;
            _startPos = new Vector2(-1, -1);
        }

        public void DrawGizmos(WorldMap map, Vector2 mouseGridPos, EditorContext ctx)
        {
            if (_isDragging)
            {
                var rect = GetRect(_startPos, mouseGridPos);
                Raylib.DrawRectangleLinesEx(rect, 2, Color.Blue);
                Raylib.DrawRectangleRec(rect, new Color(0, 0, 255, 50));
            }
        }

        private void ApplyRect(WorldMap map, Vector2 start, Vector2 end, HistoryManager history, EditorContext ctx)
        {
            var commands = new List<IEditorCommand>();
            int x1 = (int)Math.Min(start.X, end.X);
            int y1 = (int)Math.Min(start.Y, end.Y);
            int x2 = (int)Math.Max(start.X, end.X);
            int y2 = (int)Math.Max(start.Y, end.Y);

            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    commands.Add(new PlaceTileCommand(map, x, y, ctx.ActiveLayer, ctx.SelectedTileId));
                }
            }
            history.Execute(new CompositeCommand(commands));
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
