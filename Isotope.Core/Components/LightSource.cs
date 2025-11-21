using Raylib_cs;

namespace Isotope.Core.Components;

public struct LightSource
{
    public Color Color;    // Цвет света (напр. оранжевый для огня)
    public float Radius;   // Радиус круга
    public float Intensity; // Яркость
}
