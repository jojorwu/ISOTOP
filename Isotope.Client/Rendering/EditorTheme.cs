using ImGuiNET;
using System.Numerics;

namespace Isotope.Client.Rendering
{
    public static class EditorTheme
    {
        public static void Apply()
        {
            var style = ImGui.GetStyle();
            var colors = style.Colors;

            style.WindowRounding = 5.0f;
            style.FrameRounding = 4.0f;
            style.PopupRounding = 4.0f;
            style.ScrollbarRounding = 12.0f;
            style.GrabRounding = 4.0f;
            style.TabRounding = 4.0f;

            colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.15f, 0.15f, 0.15f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.15f, 0.15f, 0.15f, 1.00f);

            colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.25f, 0.29f, 1.00f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.28f, 0.56f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.06f, 0.53f, 0.98f, 1.00f);

            colors[(int)ImGuiCol.Tab] = new Vector4(0.15f, 0.15f, 0.15f, 1.00f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.38f, 0.38f, 0.38f, 1.00f);
            colors[(int)ImGuiCol.TabActive] = new Vector4(0.28f, 0.28f, 0.28f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.15f, 0.15f, 0.15f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);

            colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.26f, 0.59f, 0.98f, 0.70f);

            colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        }
    }
}
