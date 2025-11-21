using Isotope.Core.Map;

namespace Isotope.Client.Editor.Commands
{
    public class PlaceTileCommand : IEditorCommand
    {
        private WorldMap _map;
        private int _x, _y, _layer;
        private ushort _newId;
        private ushort _oldId;

        public PlaceTileCommand(WorldMap map, int x, int y, int layer, ushort newId)
        {
            _map = map;
            _x = x;
            _y = y;
            _layer = layer;
            _newId = newId;
            _oldId = map.GetTile(x, y).GetLayer(layer);
        }

        public void Execute()
        {
            _map.SetTile(_x, _y, _layer, _newId);
        }

        public void Undo()
        {
            _map.SetTile(_x, _y, _layer, _oldId);
        }
    }
}
