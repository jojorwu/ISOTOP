using Arch.Core;

namespace Isotope.Client.Editor
{
    /// <summary>
    /// A shared context object that allows different editor panels to communicate
    /// and access shared state, like the currently selected entity.
    /// </summary>

    public enum EditorMode { Object, Tile }

    public class EditorContext
    {
        /// <summary>
        /// The current editing mode (Object or Tile).
        /// </summary>
        public EditorMode CurrentMode = EditorMode.Object;

        /// <summary>
        /// The main ECS World.
        /// </summary>
        public World World;

        /// <summary>
        /// The entity currently selected in the editor.
        /// </summary>
        public Entity SelectedEntity = Entity.Null;

        /// <summary>
        /// The active tile layer for map editing (0 = Floor, 1 = Wall).
        /// </summary>
        public int ActiveLayer = 1;

        /// <summary>
        /// The ID of the tile currently selected in the palette.
        /// </summary>
        public ushort SelectedTileId = 1;

        /// <summary>
        /// Updates the currently selected entity.
        /// </summary>
        /// <param name="e">The new entity to select.</param>
        public void Select(Entity e)
        {
            SelectedEntity = e;
        }
    }
}
