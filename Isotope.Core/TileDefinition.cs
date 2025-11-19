namespace Isotope.Core;

public class TileDefinition
{
    public ushort Id;
    public string InternalName; // "wall_concrete"
    public string TexturePath;
    public bool IsSolid;
    public float ThermalConductivity; // Для атмосферы
}
