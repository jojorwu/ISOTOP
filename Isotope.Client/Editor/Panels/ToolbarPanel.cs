using ImGuiNET;
using Isotope.Client.Editor.Tools;
using System.Numerics;

namespace Isotope.Client.Editor.Panels
{
    public class ToolbarPanel
    {
        private readonly EditorContext _ctx;
        private readonly ToolManager _toolManager;

        public ToolbarPanel(EditorContext ctx, ToolManager toolManager)
        {
            _ctx = ctx;
            _toolManager = toolManager;
        }

        public void Draw()
        {
            ImGui.Begin("Tools", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize);

            foreach (var tool in _toolManager.Tools)
            {
                bool isActive = _toolManager.ActiveTool == tool;
                if (isActive)
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0.5f, 0, 1));

                if (ImGui.Button(tool.Name))
                {
                    _toolManager.SetActiveTool(tool);
                }

                if (isActive)
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
