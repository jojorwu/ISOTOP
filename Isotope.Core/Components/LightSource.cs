using Raylib_cs;

namespace Isotope.Core.Components;

/// <summary>
/// Represents a light source in the game world.
/// </summary>
public struct LightSource
{
    /// <summary>
    /// The color of the light, e.g., orange for a fire.
    /// </summary>
    public Color Color;

    /// <summary>
    /// The radius of the light circle.
    /// </summary>
    public float Radius;

    /// <summary>
    /// The intensity of the light.
    /// </summary>
    public float Intensity;
}
