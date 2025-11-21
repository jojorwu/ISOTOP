using Raylib_cs;

namespace Isotope.Core.Components;

// 2. Как объект выглядит?
public struct SpriteComponent
{
    public string TexturePath; // Путь к текстуре (как в Registry)
    public Texture2D Texture; // The loaded texture
    public Rectangle SourceRect; // Если мы режем атлас
    public Color Tint; // Цвет (белый = обычный)
}
