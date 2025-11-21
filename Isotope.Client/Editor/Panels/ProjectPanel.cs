using ImGuiNET;
using System.IO;
using System.Linq;

namespace Isotope.Client.Editor.Panels
{
    public class ProjectPanel
    {
        private readonly ScriptEditorPanel _scriptEditorPanel;
        private string _currentDirectory;

        public ProjectPanel(string rootDirectory, ScriptEditorPanel scriptEditorPanel)
        {
            _currentDirectory = rootDirectory;
            _scriptEditorPanel = scriptEditorPanel;
        }

        public void Draw()
        {
            ImGui.Begin("Project");

            var directories = Directory.GetDirectories(_currentDirectory);
            foreach (var dir in directories)
            {
                if (ImGui.Selectable(Path.GetFileName(dir) + "/", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    _currentDirectory = dir;
                }
            }

            var files = Directory.GetFiles(_currentDirectory);
            foreach (var file in files)
            {
                if (ImGui.Selectable(Path.GetFileName(file)))
                {
                    if (Path.GetExtension(file) == ".lua")
                    {
                        _scriptEditorPanel.LoadFile(file);
                    }
                }
            }

            if (ImGui.Button(".."))
            {
                var parent = Directory.GetParent(_currentDirectory);
                if (parent != null)
                {
                    _currentDirectory = parent.FullName;
                }
            }

            ImGui.End();
        }
    }
}
