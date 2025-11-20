using System;
using Arch.Core;
using Isotope.Core.Map;
using Raylib_cs;
using System.Numerics;

namespace Isotope.Client.Editor.Tools
{
    public class EyedropperTool : IEditorTool
    {
        public string Name => "Picker (I)";

        public void OnUpdate(World world, WorldMap map, Vector2 mouseGridPos, Commands.HistoryManager history, EditorContext ctx)
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                int x = (int)mouseGridPos.X;
                int y = (int)mouseGridPos.Y;

                ushort id = map.GetTile(x, y).GetLayer(ctx.ActiveLayer);

                ctx.SelectedTileId = id;

                Console.WriteLine($"Picked tile ID: {id}");
            }
        }
        public void OnDrawGizmos(WorldMap map, Vector2 mouseGridPos) { }
    }
}
