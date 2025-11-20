using ImGuiNET;
using Isotope.Client.Editor.Tools;
using System.Numerics;

namespace Isotope.Client.Editor.Panels
{
    public class ToolbarPanel
    {
        private EditorContext _ctx;
        private IEditorTool[] _tools;
        private int _activeToolIndex = 0;

        public IEditorTool ActiveTool => _tools[_activeToolIndex];

        public ToolbarPanel(EditorContext ctx)
        {
            _ctx = ctx;
            _tools = new IEditorTool[]
            {
                new BrushTool(),
                new RectTool(),
                new EyedropperTool()
            };
        }

        public void Draw()
        {
            ImGui.Begin("Tools", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize);

            for (int i = 0; i < _tools.Length; i++)
            {
                if (i == _activeToolIndex)
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0.5f, 0, 1));

                if (ImGui.Button(_tools[i].Name))
                {
                    _activeToolIndex = i;
                }

                if (i == _activeToolIndex)
                    ImGui.PopStyleColor();

                ImGui.SameLine();
            }

            ImGui.NewLine();

            ImGui.Separator();
            ImGui.Text("Active Layer:");
            if (ImGui.RadioButton("Floor", _ctx.ActiveLayer == 0)) _ctx.ActiveLayer = 0;
            ImGui.SameLine();
            if (ImGui.RadioButton("Walls", _ctx.ActiveLayer == 1)) _ctx.ActiveLayer = 1;

            ImGui.End();
        }
    }
}
