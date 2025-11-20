using System.Collections.Generic;

namespace Isotope.Client.Editor.Commands
{
    public class CompositeCommand : IEditorCommand
    {
        private readonly List<IEditorCommand> _commands = new();

        public CompositeCommand(List<IEditorCommand> commands)
        {
            _commands = commands;
        }

        public void Execute()
        {
            foreach (var cmd in _commands)
            {
                cmd.Execute();
            }
        }

        public void Undo()
        {
            // Undo in reverse order
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }
}
