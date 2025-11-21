using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Isotope.Client.Editor.Tools
{
    /// <summary>
    /// Manages the registration, selection, and lifecycle of editor tools.
    /// </summary>
    public class ToolManager
    {
        private readonly List<IEditorTool> _tools = new();
        private IEditorTool _activeTool;

        public IReadOnlyList<IEditorTool> Tools => _tools.AsReadOnly();
        public IEditorTool ActiveTool => _activeTool;

        public void DiscoverAndRegisterTools()
        {
            var toolTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IEditorTool).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in toolTypes)
            {
                try
                {
                    if (Activator.CreateInstance(type) is IEditorTool tool)
                    {
                        Register(tool);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to instantiate tool '{type.Name}': {ex.Message}");
                }
            }
        }

        private void Register(IEditorTool tool)
        {
            _tools.Add(tool);
            if (_activeTool == null)
            {
                SetActiveTool(tool);
            }
        }

        public void SetActiveTool(IEditorTool newTool)
        {
            if (_activeTool == newTool || !_tools.Contains(newTool))
            {
                return;
            }

            _activeTool?.OnDeactivate();
            _activeTool = newTool;
            _activeTool?.OnActivate();
        }
    }
}
