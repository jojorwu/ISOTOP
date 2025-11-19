// Isotope.Core/Tiles/TileRegistry.cs
namespace Isotope.Core.Tiles;

public static class TileRegistry
{
    private static readonly TileDefinition[] _definitions = new TileDefinition[65535];
    private static ushort _nextId = 1; // 0 зарезервирован под "пустоту"

    public static void Register(TileDefinition def)
    {
        def.Id = _nextId;
        _definitions[_nextId] = def;
        _nextId++;
    }

    // Метод для получения инфо о тайле по ID (супер-быстрый доступ)
    public static TileDefinition Get(ushort id)
    {
        return _definitions[id]; // O(1) доступ
    }
}
