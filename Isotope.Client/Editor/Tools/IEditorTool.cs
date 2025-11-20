using Arch.Core;
using Isotope.Core.Map;
using System.Numerics;

namespace Isotope.Client.Editor.Tools
{
    public interface IEditorTool
    {
        string Name { get; }
        void OnUpdate(World world, WorldMap map, Vector2 mouseGridPos, Commands.HistoryManager history, EditorContext ctx);
        void OnDrawGizmos(WorldMap map, Vector2 mouseGridPos);
    }
}
