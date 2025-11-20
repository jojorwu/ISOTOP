using System.Collections.Generic;

namespace Isotope.Client.Editor.Commands
{
    public class HistoryManager
    {
        private Stack<IEditorCommand> _undoStack = new();
        private Stack<IEditorCommand> _redoStack = new();

        public void Execute(IEditorCommand cmd)
        {
            cmd.Execute();
            _undoStack.Push(cmd);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var cmd = _undoStack.Pop();
                cmd.Undo();
                _redoStack.Push(cmd);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var cmd = _redoStack.Pop();
                cmd.Execute();
                _undoStack.Push(cmd);
            }
        }
    }
}
