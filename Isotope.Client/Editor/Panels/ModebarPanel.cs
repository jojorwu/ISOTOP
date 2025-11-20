using ImGuiNET;
using System.Numerics;

namespace Isotope.Client.Editor.Panels
{
    public class ModebarPanel
    {
        private EditorContext _ctx;

        public ModebarPanel(EditorContext ctx)
        {
            _ctx = ctx;
        }

        public void Draw()
        {
            ImGui.Begin("Mode", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize);

            if (DrawModeButton("Object", EditorMode.Object)) _ctx.CurrentMode = EditorMode.Object;
            ImGui.SameLine();
            if (DrawModeButton("Tile", EditorMode.Tile)) _ctx.CurrentMode = EditorMode.Tile;

            ImGui.End();
        }

        private bool DrawModeButton(string name, EditorMode mode)
        {
            if (_ctx.CurrentMode == mode)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.6f, 0.2f, 1.0f));
            }

            bool clicked = ImGui.Button(name);

            if (_ctx.CurrentMode == mode)
            {
                ImGui.PopStyleColor();
            }

            return clicked;
        }
    }
}
