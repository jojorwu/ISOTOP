using Arch.Core;
using Isotope.Core.Map;
using System.Numerics;
using Isotope.Client.Editor.Commands;

namespace Isotope.Client.Editor.Tools
{
    public interface IEditorTool
    {
        string Name { get; }

        void OnActivate();
        void OnDeactivate();

        void OnMouseDown(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx);
        void OnMouseMove(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx);
        void OnMouseUp(World world, WorldMap map, Vector2 mouseGridPos, HistoryManager history, EditorContext ctx);

        void DrawGizmos(WorldMap map, Vector2 mouseGridPos, EditorContext ctx);
    }
}
