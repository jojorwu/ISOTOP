// Isotope.Core/Tiles/TileDefinition.cs
public class TileDefinition
{
    public ushort Id { get; internal set; } // Уникальный номер (0, 1, 2...)
    public string Name { get; set; }        // Имя ("steel_wall", "grass")
    public string TexturePath { get; set; } // Путь к спрайту

    // Свойства физики
    public bool IsSolid { get; set; } = false;      // Нельзя пройти?
    public bool IsOpaque { get; set; } = false;     // Не пропускает свет?
    public float Friction { get; set; } = 1.0f;     // Скольжение (лёд < 1.0)
}
