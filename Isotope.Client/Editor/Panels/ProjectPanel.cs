using ImGuiNET;
using System.IO;

namespace Isotope.Client.Editor.Panels
{
    public class ProjectPanel
    {
        private string _currentDirectory;

        public ProjectPanel(string rootDirectory)
        {
            _currentDirectory = rootDirectory;
        }

        public void Draw()
        {
            ImGui.Begin("Project");

            // TODO: Implement file browser
            ImGui.Text("File browser placeholder.");

            ImGui.End();
        }
    }
}
