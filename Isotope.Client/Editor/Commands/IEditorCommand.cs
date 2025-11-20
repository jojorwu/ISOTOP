namespace Isotope.Client.Editor.Commands
{
    public interface IEditorCommand
    {
        void Execute();
        void Undo();
    }
}
