using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace Isotope.Client.Editor.Panels
{
    public class ConsolePanel
    {
        public static List<string> Logs = new List<string>();
        private bool _autoScroll = true;

        public static void Log(string msg) => Logs.Add($"[INFO] {msg}");
        public static void Error(string msg) => Logs.Add($"[ERROR] {msg}");

        public void Draw()
        {
            ImGui.Begin("Console");

            if (ImGui.Button("Clear")) Logs.Clear();
            ImGui.SameLine();
            ImGui.Checkbox("Auto-scroll", ref _autoScroll);

            ImGui.Separator();

            ImGui.BeginChild("ScrollingRegion", new Vector2(0, 0), false, ImGuiWindowFlags.HorizontalScrollbar);

            foreach (var log in Logs)
            {
                Vector4 color = new Vector4(1, 1, 1, 1);
                if (log.Contains("[ERROR]")) color = new Vector4(1, 0.4f, 0.4f, 1);

                ImGui.TextColored(color, log);
            }

            if (_autoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                ImGui.SetScrollHereY(1.0f);

            ImGui.EndChild();
            ImGui.End();
        }
    }
}
