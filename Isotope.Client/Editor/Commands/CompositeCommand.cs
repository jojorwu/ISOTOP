using System;
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
            // Execute is generally expected to succeed. If it fails,
            // it's a more critical error that should probably halt execution.
            foreach (var cmd in _commands)
            {
                cmd.Execute();
            }
        }

        public void Undo()
        {
            // Undo in reverse order. This operation needs to be more robust.
            // If one command fails to undo, we should still attempt to undo the others.
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                try
                {
                    _commands[i].Undo();
                }
                catch (Exception ex)
                {
                    // Log the error but continue the loop to undo the remaining commands.
                    // This prevents the editor from being left in a partially-undone, inconsistent state.
                    Console.WriteLine($"[ERROR] Failed to undo command in CompositeCommand: {ex.Message}");
                }
            }
        }
    }
}
