using Raylib_cs;

namespace Isotope.Core.Components;

/// <summary>
/// Represents the visual appearance of an entity.
/// </summary>
public struct SpriteComponent
{
    /// <summary>
    /// The path to the texture in the registry.
    /// </summary>
    public string TexturePath;

    /// <summary>
    /// The source rectangle for cutting from a texture atlas.
    /// </summary>
    public Rectangle SourceRect;

    /// <summary>
    /// The tint color, where white is the default.
    /// </summary>
    public Color Tint;

    /// <summary>
    /// A value indicating whether the sprite is visible.
    /// </summary>
    public bool Visible;
}
