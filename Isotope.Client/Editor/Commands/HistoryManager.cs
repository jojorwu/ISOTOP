using System.Collections.Generic;
using System.Linq;

namespace Isotope.Client.Editor.Commands
{
    public class HistoryManager
    {
        private const int MaxHistorySize = 100;
        private readonly LinkedList<IEditorCommand> _undoList = new();
        private readonly LinkedList<IEditorCommand> _redoList = new();

        public void Execute(IEditorCommand cmd)
        {
            cmd.Execute();
            _undoList.AddLast(cmd);

            if (_undoList.Count > MaxHistorySize)
            {
                _undoList.RemoveFirst();
            }

            _redoList.Clear();
        }

        public void Undo()
        {
            if (_undoList.Count > 0)
            {
                var cmd = _undoList.Last.Value;
                _undoList.RemoveLast();
                cmd.Undo();
                _redoList.AddLast(cmd);
            }
        }

        public void Redo()
        {
            if (_redoList.Count > 0)
            {
                var cmd = _redoList.Last.Value;
                _redoList.RemoveLast();
                cmd.Execute();
                _undoList.AddLast(cmd);
            }
        }
    }
}
