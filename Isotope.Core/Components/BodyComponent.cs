using System.Numerics;

namespace Isotope.Core.Components;

// 3. Физика (твердое тело)
public struct BodyComponent
{
    public Vector2 Velocity; // Текущая скорость
    public Vector2 Size;     // Размер хитбокса (например, 16x16)
    public bool IsStatic;    // Если true - это стена или прикрученный стол
}
