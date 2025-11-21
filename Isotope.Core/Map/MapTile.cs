namespace Isotope.Core.Map;

public struct MapTile
{
    public ushort FloorId;
    public ushort WallId;

    public ushort GetLayer(int layer)
    {
        return layer == 0 ? FloorId : WallId;
    }

    public void SetLayer(int layer, ushort tileId)
    {
        if (layer == 0)
        {
            FloorId = tileId;
        }
        else
        {
            WallId = tileId;
        }
    }
}
