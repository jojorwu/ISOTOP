using ImGuiNET;
using System.IO;
using System.Numerics;

namespace Isotope.Client.Editor.Panels
{
    /// <summary>
    /// A panel that provides a simple in-editor text editor for scripts.
    /// </summary>
    public class ScriptEditorPanel
    {
        private string _currentFilePath;
        private string _text = "";
        private bool _unsavedChanges = false;

        public void Draw()
        {
            ImGui.Begin("Script Editor");

            if (ImGui.Button("Save"))
            {
                Save();
            }
            ImGui.SameLine();
            if (_unsavedChanges)
            {
                ImGui.Text("*");
            }

            ImGui.Separator();

            if (ImGui.InputTextMultiline("##source", ref _text, 100000, new Vector2(-1, -1), ImGuiInputTextFlags.AllowTabInput))
            {
                _unsavedChanges = true;
            }

            ImGui.End();
        }

        public void LoadFile(string filePath)
        {
            if (_unsavedChanges)
            {
                // In a real editor, we'd prompt the user to save.
                // For now, we'll just discard the changes.
                System.Console.WriteLine($"[WARNING] Discarding unsaved changes in {_currentFilePath}");
            }

            try
            {
                _text = File.ReadAllText(filePath);
                _currentFilePath = filePath;
                _unsavedChanges = false;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"[ERROR] Failed to load script file: {e.Message}");
                _text = $"-- Failed to load {filePath} --";
                _currentFilePath = null;
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                System.Console.WriteLine("[ERROR] No file is open to save.");
                return;
            }

            try
            {
                File.WriteAllText(_currentFilePath, _text);
                _unsavedChanges = false;
                System.Console.WriteLine($"[INFO] Saved script: {_currentFilePath}");
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"[ERROR] Failed to save script file: {e.Message}");
            }
        }
    }
}
